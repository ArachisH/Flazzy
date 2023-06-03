using Flazzy.ABC;

namespace Flazzy.Tools;

public class AS3MultinameUpgrader
{
    private readonly Dictionary<string, string> _namespaces;
    private readonly Dictionary<string, string> _namespacesUpgraded;

    private readonly Dictionary<string, string> _qualifiedNames;
    private readonly Dictionary<string, string> _qualifiedNamesUpgraded;

    public AS3MultinameUpgrader()
    {
        _namespaces = new Dictionary<string, string>();
        _namespacesUpgraded = new Dictionary<string, string>();

        _qualifiedNames = new Dictionary<string, string>();
        _qualifiedNamesUpgraded = new Dictionary<string, string>();
    }

    public int Search(ABCFile abc, bool isAddingMetadata = true)
    {
        int metadataNameIndex = isAddingMetadata ? abc.Pool.AddConstant("Flazzy", false) : 0;
        int previousNamespaceIndex = isAddingMetadata ? abc.Pool.AddConstant("PreviousNamespace", false) : 0;
        int previousQualifiedNameIndex = isAddingMetadata ? abc.Pool.AddConstant("PreviousQualifiedName", false) : 0;

        int namesUpgraded = 0;
        string upgradedNamespaceName = null;
        string upgradedClassQualifiedName = null;
        foreach (ASScript script in abc.Scripts)
        {
            foreach (ASTrait trait in script.Traits)
            {
                if (trait.Kind != TraitKind.Class) continue;

                ASClass @class = trait.Class;
                if (!SearchClass(@class, ref upgradedNamespaceName, ref upgradedClassQualifiedName)) continue;

                namesUpgraded++;
                ASMetadata metadata = null;
                if (isAddingMetadata)
                {
                    trait.Attributes |= TraitAttributes.Metadata;

                    metadata = new ASMetadata(abc);
                    metadata.NameIndex = metadataNameIndex;

                    trait.MetadataIndices.Add(abc.AddMetadata(metadata, false));
                }

                if (!string.IsNullOrWhiteSpace(upgradedNamespaceName))
                {
                    if (isAddingMetadata)
                    {
                        metadata.Items.Add(new ASItemInfo(abc, previousNamespaceIndex, abc.Pool.AddConstant(@class.QName.Namespace.Name, false)));
                    }
                    abc.Pool.Strings[trait.QName.Namespace.NameIndex] = upgradedNamespaceName;
                }

                if (!string.IsNullOrWhiteSpace(upgradedClassQualifiedName))
                {
                    if (isAddingMetadata)
                    {
                        metadata.Items.Add(new ASItemInfo(abc, previousQualifiedNameIndex, abc.Pool.AddConstant(@class.QName.Name, false)));
                    }
                    abc.Pool.Strings[trait.QName.NameIndex] = upgradedClassQualifiedName;
                }

                if (@class.Instance.Flags.HasFlag(ClassFlags.ProtectedNamespace))
                {
                    abc.Pool.Strings[@class.Instance.ProtectedNamespace.NameIndex] = $"{trait.QName.Namespace.Name}:{trait.QName.Name}";
                }
            }
        }
        return namesUpgraded;
    }

    private bool SearchClass(ASClass @class, ref string namespaceNameUpgrade, ref string qualifiedNameUpgrade)
    {
        static void UpdateCache(Dictionary<string, string> byPrevious, Dictionary<string, string> byCurrent, string previous, string current, ref bool hasUpgradedFlag)
        {
            // Indicates that the cache has already been updated with the 'current' parameter, or that the 'current' parameter is not valid for caching.
            if (hasUpgradedFlag || string.IsNullOrWhiteSpace(current)) return;

            byPrevious.Add(previous, current);
            byCurrent.Add(current, previous);
            hasUpgradedFlag = true;
        }

        // Clear name references, and attempt to pull already upgraded names.
        bool hasUpgradedNamespace = _namespaces.TryGetValue(@class.QName.Namespace.Name, out namespaceNameUpgrade);
        bool hasUpgradedQualifiedName = _qualifiedNames.TryGetValue(@class.QName.Name, out qualifiedNameUpgrade);
        if (hasUpgradedNamespace && hasUpgradedQualifiedName) return true;

        if (_namespacesUpgraded.ContainsKey(@class.QName.Namespace.Name))
        {
            hasUpgradedNamespace = true;
            namespaceNameUpgrade = @class.QName.Namespace.Name;
        }
        if (_qualifiedNamesUpgraded.ContainsKey(@class.QName.Name))
        {
            hasUpgradedQualifiedName = true;
            qualifiedNameUpgrade = @class.QName.Name;
        }

        ASInstance instance = @class.Instance;
        /* -------- Resolve by Trait(s) -------- */
        foreach (ASTrait trait in @class.Traits.Concat(instance.Traits))
        {
            if (SearchTrait(@class, trait, ref namespaceNameUpgrade, ref qualifiedNameUpgrade)) break;
            // TODO: If only one of the names is found in the trait, should we check the instructions as well for the second name?

            ASMethod method = trait.Method ?? trait.Function;
            if (method == null || !IsSearchingMethod(@class, trait)) continue; // Trait has no method, or trait not desirable for searching.

            /* -------- Resolve by Instruction(s) -------- */
            if (SearchByInstruction(@class, method, ref namespaceNameUpgrade, ref qualifiedNameUpgrade)) break;
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
        return TryUpgrade(_namespacesUpgraded, @class.QName.Namespace.Name, left, ref namespaceNameUpgrade) || TryUpgrade(_qualifiedNamesUpgraded, @class.QName.Name, right, ref qualifiedNameUpgrade);
    }

    protected virtual bool IsSearchingMethod(ASClass @class, ASTrait trait) => false;
    protected virtual bool SearchByInstruction(ASClass @class, ASMethod method, ref string namespaceNameUpgrade, ref string qualifiedNameUpgrade)
    {
        /*
         * As a last resort, if no name has been successfully resolved, we can attempt to extract the fully qualified name from an instruction attempting to resolve the current instance/scope.
         * If an internal method like 'toString' is called on a locally scoped/initialized variable(try/catch, switch, etc), it may utilize the real fully qualified name of the class when invoking the internal method call.
         * setproperty MultinameL([ !!>>> PrivateNamespace("com.hurlant.util:Hex") <<<!! ,StaticProtectedNs("com.hurlant.util:Hex"),StaticProtectedNs("Object"),PackageNamespace("com.hurlant.util"),PackageInternalNs("com.hurlant.util"),PrivateNamespace("FilePrivateNS:Hex"),PackageNamespace(""),Namespace("http://adobe.com/AS3/2006/builtin")])
         */

        return false;
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
    private static bool TryUpgrade(Dictionary<string, string> byCurrent, ReadOnlySpan<char> previous, ReadOnlySpan<char> current, ref string nameUpgrade)
    {
        // Upgrade has already been applied.
        if (!string.IsNullOrWhiteSpace(nameUpgrade)) return false;

        // Names already match, nothing to upgrade.
        if (current.Equals(previous, StringComparison.OrdinalIgnoreCase)) return false;

        nameUpgrade = current.ToString();
        return true;
    }
}