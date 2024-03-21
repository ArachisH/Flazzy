using Flazzy.ABC;

namespace Flazzy.Tools;

public class AS3MultinameUpgrader
{
    private readonly bool _isApplyingMetadata;
    private readonly bool _isParsingInstructions;

    private readonly Dictionary<string, string> _oldClassNames, _newClassNames;
    private readonly Dictionary<string, string> _oldNamespaceNames, _newNamespaceNames;

    public AS3MultinameUpgrader(bool isApplyingMetadata, bool isParsingInstructions)
    {
        _isApplyingMetadata = isApplyingMetadata;
        _isParsingInstructions = isParsingInstructions;

        _oldClassNames = new Dictionary<string, string>();
        _newClassNames = new Dictionary<string, string>();

        _oldNamespaceNames = new Dictionary<string, string>();
        _newNamespaceNames = new Dictionary<string, string>();
    }

    public int Search(ABCFile abc)
    {
        int metadataNameIndex = _isApplyingMetadata ? abc.Pool.AddConstant("Flazzy", false) : 0;
        int previousNamespaceIndex = _isApplyingMetadata ? abc.Pool.AddConstant("PreviousNamespace", false) : 0;
        int previousQualifiedNameIndex = _isApplyingMetadata ? abc.Pool.AddConstant("PreviousQualifiedName", false) : 0;

        int namesUpgraded = 0;
        string newClassName = null;
        string newNamespaceName = null;
        foreach (ASTrait trait in abc.Scripts.SelectMany(s => s.Traits))
        {
            if (trait.Kind != TraitKind.Class) continue;

            ASClass @class = trait.Class;
            if (!SearchClass(@class, ref newNamespaceName, ref newClassName, out string oldNamespaceName, out string oldClassName)) continue;

            namesUpgraded++;
            ASMetadata metadata = null;
            if (_isApplyingMetadata)
            {
                trait.Attributes |= TraitAttributes.Metadata;

                metadata = new ASMetadata(abc)
                {
                    NameIndex = metadataNameIndex
                };

                trait.MetadataIndices.Add(abc.AddMetadata(metadata, false));
            }

            if (!string.IsNullOrWhiteSpace(newNamespaceName))
            {
                if (_isApplyingMetadata)
                {
                    metadata.Items.Add(new ASItemInfo(abc, previousNamespaceIndex, abc.Pool.AddConstant(oldNamespaceName, false)));
                }
                if (trait.QName.Namespace.Name != newNamespaceName)
                {
                    abc.Pool.Strings[trait.QName.Namespace.NameIndex] = newNamespaceName;
                }
            }

            if (!string.IsNullOrWhiteSpace(newClassName))
            {
                if (_isApplyingMetadata)
                {
                    metadata.Items.Add(new ASItemInfo(abc, previousQualifiedNameIndex, abc.Pool.AddConstant(oldClassName, false)));
                }
                if (trait.QName.Name != newClassName)
                {
                    abc.Pool.Strings[trait.QName.NameIndex] = newClassName;
                }
            }

            if (@class.Instance.Flags.HasFlag(ClassFlags.ProtectedNamespace))
            {
                string protectedNamespaceUpgrade = $"{trait.QName.Namespace.Name}:{trait.QName.Name}";
                _newNamespaceNames.Add(@class.Instance.ProtectedNamespace.Name, protectedNamespaceUpgrade);
                abc.Pool.Strings[@class.Instance.ProtectedNamespace.NameIndex] = protectedNamespaceUpgrade;
            }
        }
        SearchMultinames(abc);
        return namesUpgraded;
    }

    private void SearchMultinames(ABCFile abc)
    {
        string newNamespaceName;
        string newClassName = null;
        foreach (ASMultiname multiname in abc.Pool.Multinames)
        {
            if (multiname == null) continue;
            if (multiname.NamespaceSetIndex != 0)
            {
                foreach (ASNamespace @namespace in multiname.NamespaceSet.GetNamespaces())
                {
                    if (_newNamespaceNames.TryGetValue(@namespace.Name, out newNamespaceName))
                    {
                        abc.Pool.Strings[@namespace.NameIndex] = newNamespaceName;
                    }
                    else if (TryParseNamespace(@namespace.Name, out ReadOnlySpan<char> left, out ReadOnlySpan<char> right))
                    {
                        // TODO: Come back to avoid string allocations if this feature is implemented https://github.com/dotnet/runtime/issues/27229
                        string leftPart = left.ToString();
                        string rightPart = right.ToString();

                        if (AccessCache(_oldNamespaceNames, _newNamespaceNames, leftPart, out string oldNamespaceName, out newNamespaceName))
                        {
                            if (_newClassNames.TryGetValue($"{oldNamespaceName}.{right}", out string newFullClassName))
                            {
                                newClassName = GetClassName(newFullClassName);
                            }
                            abc.Pool.Strings[@namespace.NameIndex] = $"{newNamespaceName ?? leftPart}:{newClassName ?? rightPart}";
                        }
                    }
                }
            }

            if (multiname.NamespaceIndex != 0 && _newNamespaceNames.TryGetValue(multiname.Namespace.Name, out newNamespaceName))
            {
                abc.Pool.Strings[multiname.Namespace.NameIndex] = newNamespaceName;
            }

            if (multiname.NameIndex != 0)
            {
                string oldNamespaceName = multiname.Namespace?.Name ?? string.Empty;
                if (!_oldNamespaceNames.TryGetValue(oldNamespaceName, out oldNamespaceName))
                {
                    oldNamespaceName = multiname.Namespace?.Name;
                }

                string fullOldClassName = multiname.Name;
                if (!string.IsNullOrWhiteSpace(oldNamespaceName))
                {
                    fullOldClassName = $"{oldNamespaceName}.{multiname.Name}";
                }

                if (_newClassNames.TryGetValue(fullOldClassName, out newClassName))
                {
                    int lastDot = newClassName.LastIndexOf('.');
                    abc.Pool.Strings[multiname.NameIndex] = newClassName.Substring(lastDot + 1);
                }
            }
        }
    }
    private bool SearchClass(ASClass @class, ref string newNamespaceName, ref string newClassName, out string oldNamespaceName, out string oldClassName)
    {
        bool isNewNamespaceNameCached = AccessCache(_oldNamespaceNames, _newNamespaceNames, @class.QName.Namespace.Name, out oldNamespaceName, out newNamespaceName);

        // Namespace must be checked first, to ensure that we're using the old full qualified class name.
        newClassName = null;
        oldClassName = @class.QName.Name;
        string oldFullClassName = oldClassName;
        if (!string.IsNullOrWhiteSpace(oldNamespaceName))
        {
            oldFullClassName = $"{oldNamespaceName}.{@class.QName.Name}";
        }

        bool isNewClassNameCached = AccessCache(_oldClassNames, _newClassNames, oldFullClassName, out oldFullClassName, out string newFullClassName);
        if (isNewClassNameCached) // Separate the namespace from the full qualified class name.
        {
            oldClassName = GetClassName(oldFullClassName);
            newClassName = GetClassName(newFullClassName);
        }

        // New names do exist, no need to search through the traits.
        if (isNewNamespaceNameCached && isNewClassNameCached) return true;

        ASInstance instance = @class.Instance;
        /* -------- Resolve by Trait(s) -------- */
        foreach (ASTrait trait in @class.Traits.Concat(instance.Traits))
        {
            if (SearchTrait(@class, trait, ref newNamespaceName, ref newClassName) && !_isParsingInstructions) break;
            if (!string.IsNullOrWhiteSpace(newNamespaceName) && !string.IsNullOrWhiteSpace(newClassName)) break;

            ASMethod method = trait.Method ?? trait.Function;
            if (method == null || method.Body == null) continue;

            /* -------- Resolve by Instruction(s) -------- */
            if (SearchInstructions(@class, method, ref newNamespaceName, ref newClassName)) break;
        }

        /* -------- Resolve by Instance Constructor -------- */
        // Check if the constructor name isn't already the same as the class name, and only check the ending of the constructor name as it may have a package name prefix.
        if (string.IsNullOrWhiteSpace(newClassName) && !string.IsNullOrWhiteSpace(instance.Constructor.Name) &&
            !instance.Constructor.Name.EndsWith(@class.QName.Name, StringComparison.OrdinalIgnoreCase))
        {
            newClassName = instance.Constructor.Name;
        }

        if (!isNewClassNameCached && !string.IsNullOrWhiteSpace(newClassName))
        {
            newFullClassName = $"{oldNamespaceName}.{newClassName}";
        }

        isNewClassNameCached = isNewClassNameCached || TryUpdateCache(_oldClassNames, _newClassNames, oldFullClassName, newFullClassName);
        isNewNamespaceNameCached = isNewNamespaceNameCached || TryUpdateCache(_oldNamespaceNames, _newNamespaceNames, @class.QName.Namespace.Name, newNamespaceName);
        return isNewNamespaceNameCached || isNewClassNameCached;
    }

    protected virtual bool SearchTrait(ASClass @class, ASTrait trait, ref string namespaceNameUpgrade, ref string qualifiedNameUpgrade)
    {
        // '*' ?
        if (trait.QName.Namespace.Name.Length < 2) return false;
        if (trait.QName.Namespace.Kind == NamespaceKind.PackageInternal) return false;

        ReadOnlySpan<char> traitNamespaceName = trait.QName.Namespace.Name;
        if (traitNamespaceName.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;

        if (!TryParseNamespace(trait.QName.Namespace.Name, out ReadOnlySpan<char> left, out ReadOnlySpan<char> right)) return false;

        // Return true only if any names were upgraded from this trait.
        bool wasQualifiedNameUpgraded = TryUpgrade(@class.QName.Name, right, ref qualifiedNameUpgrade);
        bool wasNamespaceNameUpgraded = TryUpgrade(@class.QName.Namespace.Name, left, ref namespaceNameUpgrade);
        return wasQualifiedNameUpgraded || wasNamespaceNameUpgraded;
    }
    protected virtual bool SearchInstructions(ASClass @class, ASMethod method, ref string namespaceNameUpgrade, ref string qualifiedNameUpgrade)
    {
        /*
         * As a last resort, if no name has been successfully resolved, we can attempt to extract the fully qualified name from an instruction attempting to resolve the current instance/scope.
         * If an internal method like 'toString' is called on a locally scoped/initialized variable(try/catch, switch, etc), it may utilize the real fully qualified name of the class when invoking the internal method call.
         * setproperty MultinameL([ !!>>> PrivateNamespace("com.hurlant.util:Hex") <<<!! ,StaticProtectedNs("com.hurlant.util:Hex"),StaticProtectedNs("Object"),PackageNamespace("com.hurlant.util"),PackageInternalNs("com.hurlant.util"),PrivateNamespace("FilePrivateNS:Hex"),PackageNamespace(""),Namespace("http://adobe.com/AS3/2006/builtin")])
         */

        return false;
    }

    private static bool TryUpgrade(ReadOnlySpan<char> previous, ReadOnlySpan<char> current, ref string nameUpgrade)
    {
        // Upgrade has already been applied.
        if (!string.IsNullOrWhiteSpace(nameUpgrade)) return false;

        // Names already match, nothing to upgrade.
        if (current.Equals(previous, StringComparison.OrdinalIgnoreCase)) return false;

        nameUpgrade = current.ToString();
        return true;
    }
    private static bool TryParseNamespace(ReadOnlySpan<char> fullName, out ReadOnlySpan<char> left, out ReadOnlySpan<char> right)
    {
        left = default;
        right = default;

        // Internal AS3 method
        if (fullName.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;

        // File scoped class
        if (fullName.StartsWith("FilePrivateNS:")) return false;

        int separatorIndex = fullName.IndexOf(':');
        if (separatorIndex == -1) return false;

        left = fullName.Slice(0, separatorIndex);
        right = fullName.Slice(separatorIndex + 1);
        return true;
    }

    private static string GetClassName(string fullClassName)
    {
        if (string.IsNullOrWhiteSpace(fullClassName)) return fullClassName;

        int dotIndex = fullClassName.LastIndexOf('.');
        return dotIndex != -1 ? fullClassName.Substring(dotIndex + 1) : fullClassName;
    }
    private static bool TryUpdateCache(Dictionary<string, string> oldNames, Dictionary<string, string> newNames, string oldName, string newName)
    {
        // Indicates that the cache has already been updated with the 'current' parameter, or that the 'current' parameter is not valid for caching.
        if (string.IsNullOrWhiteSpace(newName)) return false;

        oldNames.Add(newName, oldName);
        newNames.Add(oldName, newName);
        return true;
    }
    private static bool AccessCache(Dictionary<string, string> oldNames, Dictionary<string, string> newNames, string name, out string oldName, out string newName)
    {
        oldName = name;
        if (string.IsNullOrWhiteSpace(name))
        {
            newName = null;
            return false;
        }

        // Check if the current name has an existing update from a previous search.
        // If it does not, then check if the name was updated implicitly through the string constant pool.
        if (!newNames.TryGetValue(name, out newName) && oldNames.TryGetValue(name, out string cachedOldName))
        {
            newName = name;
            oldName = cachedOldName;
        }

        return !string.IsNullOrWhiteSpace(newName);
    }
}