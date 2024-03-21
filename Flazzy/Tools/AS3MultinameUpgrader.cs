using Flazzy.ABC;

namespace Flazzy.Tools;

public class AS3MultinameUpgrader
{
    private readonly bool _isApplyingMetadata;
    private readonly bool _isParsingInstructions;

    private readonly HashSet<string> _namespacesUpgraded;
    private readonly Dictionary<string, string> _namespaces;

    private readonly HashSet<string> _qualifiedNamesUpgraded;
    private readonly Dictionary<string, string> _qualifiedNames;

    public AS3MultinameUpgrader(bool isApplyingMetadata, bool isParsingInstructions)
    {
        _isApplyingMetadata = isApplyingMetadata;
        _isParsingInstructions = isParsingInstructions;

        _namespacesUpgraded = new HashSet<string>();
        _namespaces = new Dictionary<string, string>();

        _qualifiedNamesUpgraded = new HashSet<string>();
        _qualifiedNames = new Dictionary<string, string>();
    }

    public int Search(ABCFile abc)
    {
        int metadataNameIndex = _isApplyingMetadata ? abc.Pool.AddConstant("Flazzy", false) : 0;
        int previousNamespaceIndex = _isApplyingMetadata ? abc.Pool.AddConstant("PreviousNamespace", false) : 0;
        int previousQualifiedNameIndex = _isApplyingMetadata ? abc.Pool.AddConstant("PreviousQualifiedName", false) : 0;

        int namesUpgraded = 0;
        string upgradedNamespaceName = null;
        string upgradedClassQualifiedName = null;
        foreach (ASTrait trait in abc.Scripts.SelectMany(s => s.Traits))
        {
            if (trait.Kind != TraitKind.Class) continue;

            ASClass @class = trait.Class;
            if (!SearchClass(@class, ref upgradedNamespaceName, ref upgradedClassQualifiedName)) continue;

            namesUpgraded++;
            ASMetadata metadata = null;
            if (_isApplyingMetadata)
            {
                trait.Attributes |= TraitAttributes.Metadata;

                metadata = new ASMetadata(abc);
                metadata.NameIndex = metadataNameIndex;

                trait.MetadataIndices.Add(abc.AddMetadata(metadata, false));
            }

            if (!string.IsNullOrWhiteSpace(upgradedNamespaceName))
            {
                if (_isApplyingMetadata)
                {
                    metadata.Items.Add(new ASItemInfo(abc, previousNamespaceIndex, abc.Pool.AddConstant(@class.QName.Namespace.Name, false)));
                }
                abc.Pool.Strings[trait.QName.Namespace.NameIndex] = upgradedNamespaceName;
            }

            if (!string.IsNullOrWhiteSpace(upgradedClassQualifiedName))
            {
                if (_isApplyingMetadata)
                {
                    metadata.Items.Add(new ASItemInfo(abc, previousQualifiedNameIndex, abc.Pool.AddConstant(@class.QName.Name, false)));
                }
                abc.Pool.Strings[trait.QName.NameIndex] = upgradedClassQualifiedName;
            }

            if (@class.Instance.Flags.HasFlag(ClassFlags.ProtectedNamespace))
            {
                string protectedNamespaceUpgrade = $"{trait.QName.Namespace.Name}:{trait.QName.Name}";
                _namespaces.Add(@class.Instance.ProtectedNamespace.Name, protectedNamespaceUpgrade);
                abc.Pool.Strings[@class.Instance.ProtectedNamespace.NameIndex] = protectedNamespaceUpgrade;
            }
        }
        SearchMultinames(abc);
        return namesUpgraded;
    }

    private void SearchMultinames(ABCFile abc)
    {
        string namespaceUpgrade;
        string qualifiedNameUpgrade = null;
        foreach (ASMultiname multiname in abc.Pool.Multinames)
        {
            if (multiname == null) continue;

            if (multiname.NamespaceSetIndex != 0)
            {
                foreach (ASNamespace @namespace in multiname.NamespaceSet.GetNamespaces())
                {
                    if (_namespaces.TryGetValue(@namespace.Name, out namespaceUpgrade))
                    {
                        abc.Pool.Strings[@namespace.NameIndex] = namespaceUpgrade;
                    }
                    else if (TryParseNamespace(@namespace.Name, out ReadOnlySpan<char> left, out ReadOnlySpan<char> right))
                    {
                        // TODO: Come back to avoid string allocations if this feature is implemented https://github.com/dotnet/runtime/issues/27229
                        string leftPart = left.ToString();
                        string rightPart = right.ToString();

                        // Check if neither names can be upgraded
                        if (!_namespaces.TryGetValue(leftPart, out namespaceUpgrade) && !_qualifiedNames.TryGetValue(rightPart, out qualifiedNameUpgrade)) continue;

                        abc.Pool.Strings[@namespace.NameIndex] = $"{namespaceUpgrade ?? leftPart}:{qualifiedNameUpgrade ?? rightPart}";
                    }
                }
            }

            if (multiname.NamespaceIndex != 0 && _namespaces.TryGetValue(multiname.Namespace.Name, out namespaceUpgrade))
            {
                abc.Pool.Strings[multiname.Namespace.NameIndex] = namespaceUpgrade;
            }

            if (multiname.NameIndex != 0 && _qualifiedNames.TryGetValue(multiname.Name, out qualifiedNameUpgrade))
            {
                abc.Pool.Strings[multiname.NameIndex] = qualifiedNameUpgrade;
            }
        }
    }
    private bool SearchClass(ASClass @class, ref string namespaceNameUpgrade, ref string qualifiedNameUpgrade)
    {
        static void UpdateCache(Dictionary<string, string> byPrevious, HashSet<string> byCurrent, string previous, string current, ref bool hasUpgradedFlag)
        {
            // Indicates that the cache has already been updated with the 'current' parameter, or that the 'current' parameter is not valid for caching.
            if (hasUpgradedFlag || string.IsNullOrWhiteSpace(current)) return;

            byCurrent.Add(current);
            byPrevious.Add(previous, current);
            hasUpgradedFlag = true;
        }

        // Clear name references, and attempt to pull already upgraded names.
        bool hasUpgradedNamespace = _namespaces.TryGetValue(@class.QName.Namespace.Name, out namespaceNameUpgrade);
        bool hasUpgradedQualifiedName = _qualifiedNames.TryGetValue(@class.QName.Name, out qualifiedNameUpgrade);
        if (hasUpgradedNamespace && hasUpgradedQualifiedName) return true;

        if (_namespacesUpgraded.Contains(@class.QName.Namespace.Name))
        {
            hasUpgradedNamespace = true;
            namespaceNameUpgrade = @class.QName.Namespace.Name;
        }
        if (_qualifiedNamesUpgraded.Contains(@class.QName.Name))
        {
            hasUpgradedQualifiedName = true;
            qualifiedNameUpgrade = @class.QName.Name;
        }

        ASInstance instance = @class.Instance;
        /* -------- Resolve by Trait(s) -------- */
        foreach (ASTrait trait in @class.Traits.Concat(instance.Traits))
        {
            if (SearchTrait(@class, trait, ref namespaceNameUpgrade, ref qualifiedNameUpgrade) && !_isParsingInstructions) break;
            if (!string.IsNullOrWhiteSpace(namespaceNameUpgrade) && !string.IsNullOrWhiteSpace(qualifiedNameUpgrade)) break;

            ASMethod method = trait.Method ?? trait.Function;
            if (method == null || method.Body == null) continue;

            /* -------- Resolve by Instruction(s) -------- */
            if (SearchInstructions(@class, method, ref namespaceNameUpgrade, ref qualifiedNameUpgrade)) break;
        }

        /* -------- Resolve by Instance Constructor -------- */
        // Check if the constructor name isn't already the same as the class name, and only check the ending of the constructor name as it may have a package name prefix.
        if (string.IsNullOrWhiteSpace(qualifiedNameUpgrade) &&
            !string.IsNullOrWhiteSpace(instance.Constructor.Name) &&
            !instance.Constructor.Name.EndsWith(@class.QName.Name, StringComparison.OrdinalIgnoreCase))
        {
            qualifiedNameUpgrade = instance.Constructor.Name;
        }

        UpdateCache(_namespaces, _namespacesUpgraded, @class.QName.Namespace.Name, namespaceNameUpgrade, ref hasUpgradedNamespace);
        UpdateCache(_qualifiedNames, _qualifiedNamesUpgraded, @class.QName.Name, qualifiedNameUpgrade, ref hasUpgradedQualifiedName);

        return hasUpgradedNamespace || hasUpgradedQualifiedName;
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
}