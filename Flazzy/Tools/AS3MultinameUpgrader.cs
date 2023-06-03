using Flazzy.ABC;

namespace Flazzy.Tools;

public class AS3MultinameUpgrader
{
    public int Search(ABCFile abc, bool isAddingMetadata = true)
    {
        int metadataNameIndex = isAddingMetadata ? abc.Pool.AddConstant("Flazzy", false) : 0;
        int previousNamespaceIndex = isAddingMetadata ? abc.Pool.AddConstant("PreviousNamespace", false) : 0;
        int previousQualifiedNameIndex = isAddingMetadata ? abc.Pool.AddConstant("PreviousQualifiedName", false) : 0;

        int namesUpgraded = 0;
        string upgradedNamespaceName;
        string upgradedClassQualifiedName;
        foreach (ASScript script in abc.Scripts)
        {
            foreach (ASTrait trait in script.Traits)
            {
                if (trait.Kind != TraitKind.Class) continue;

                ASClass @class = trait.Class;
                if (SearchClass(@class, out upgradedNamespaceName, out upgradedClassQualifiedName))
                {
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
                            metadata.Items.Add(new ASItemInfo(abc, previousNamespaceIndex, @class.QName.Namespace.NameIndex));
                        }
                        trait.QName.Namespace.NameIndex = abc.Pool.AddConstant(upgradedNamespaceName);
                    }

                    if (!string.IsNullOrWhiteSpace(upgradedClassQualifiedName))
                    {
                        if (isAddingMetadata)
                        {
                            metadata.Items.Add(new ASItemInfo(abc, previousQualifiedNameIndex, @class.QName.NameIndex));
                        }
                        trait.QName.NameIndex = abc.Pool.AddConstant(upgradedClassQualifiedName);
                    }

                    if (@class.Instance.Flags.HasFlag(ClassFlags.ProtectedNamespace))
                    {
                        abc.Pool.Strings[@class.Instance.ProtectedNamespace.NameIndex] = $"{trait.QName.Namespace.Name}:{trait.QName.Name}";
                    }
                }
            }
        }
        return namesUpgraded;
    }

    private bool SearchClass(ASClass @class, out string upgradedNamespaceName, out string upgradedClassQualifiedName)
    {
        upgradedNamespaceName = null;
        upgradedClassQualifiedName = null;
        ASInstance instance = @class.Instance;

        /* -------- Resolve by Trait(s) -------- */
        foreach (ASTrait trait in @class.Traits.Concat(instance.Traits))
        {
            if (SearchByTrait(@class, trait, out upgradedNamespaceName, out upgradedClassQualifiedName))
                return true;

            ASMethod method = trait.Method ?? trait.Function;
            if (method == null || !IsSearchingMethod(@class, trait)) continue; // Trait has no method, or trait not desirable for searching.

            /* -------- Resolve by Instruction(s) -------- */
            if (SearchByInstruction(@class, method, out upgradedNamespaceName, out upgradedClassQualifiedName)) return true;
        }

        /* -------- Resolve by Instance Constructor -------- */
        // Check if the constructor name isn't already the same as the class name, and only check the name end of the constructor as it may have a prefix of 'package/*'.
        if (!string.IsNullOrWhiteSpace(instance.Constructor.Name) && !instance.Constructor.Name.EndsWith(instance.QName.Name, StringComparison.OrdinalIgnoreCase))
        {
            upgradedClassQualifiedName = instance.Constructor.Name;
        }

        return !string.IsNullOrWhiteSpace(upgradedNamespaceName) || !string.IsNullOrWhiteSpace(upgradedClassQualifiedName);
    }

    protected virtual bool SearchByTrait(ASClass @class, ASTrait trait, out string upgradedNamespaceName, out string upgradedClassQualifiedName)
    {
        upgradedNamespaceName = null;
        upgradedClassQualifiedName = null;

        // '*' ?
        if (trait.QName.Namespace.Name.Length <= 1) return false;
        if (trait.QName.Namespace.Kind == NamespaceKind.PackageInternal) return false;

        ReadOnlySpan<char> traitNamespaceName = trait.QName.Namespace.Name;
        if (traitNamespaceName.StartsWith("http", StringComparison.OrdinalIgnoreCase)) return false;

        int separatorIndex = traitNamespaceName.IndexOf(':');
        if (separatorIndex == -1) return false;

        ReadOnlySpan<char> traitNamespaceNameLeft = traitNamespaceName.Slice(0, separatorIndex);
        ReadOnlySpan<char> traitNamespaceNameRight = traitNamespaceName.Slice(separatorIndex + 1);

        if (!traitNamespaceNameLeft.Equals(@class.QName.Namespace.Name, StringComparison.OrdinalIgnoreCase))
        {
            upgradedNamespaceName = traitNamespaceNameLeft.ToString();
        }

        if (!traitNamespaceNameRight.Equals(@class.QName.Name, StringComparison.OrdinalIgnoreCase))
        {
            upgradedClassQualifiedName = traitNamespaceNameRight.ToString();
        }

        return !string.IsNullOrWhiteSpace(upgradedNamespaceName) || !string.IsNullOrWhiteSpace(upgradedClassQualifiedName);
    }

    protected virtual bool IsSearchingMethod(ASClass @class, ASTrait trait) => false;
    protected virtual bool SearchByInstruction(ASClass @class, ASMethod method, out string upgradedNamespaceName, out string upgradedClassQualifiedName)
    {
        upgradedNamespaceName = null;
        upgradedClassQualifiedName = null;

        /*
         * As a last resort, if no name has been successfully resolved, we can attempt to extract the fully qualified name from an instruction attempting to resolve the current instance/scope.
         * If an internal method like 'toString' is called on a locally scoped/initialized variable(try/catch, switch, etc), it may utilize the real fully qualified name of the class when invoking the internal method call.
         * setproperty MultinameL([ !!>>> PrivateNamespace("com.hurlant.util:Hex") <<<!! ,StaticProtectedNs("com.hurlant.util:Hex"),StaticProtectedNs("Object"),PackageNamespace("com.hurlant.util"),PackageInternalNs("com.hurlant.util"),PrivateNamespace("FilePrivateNS:Hex"),PackageNamespace(""),Namespace("http://adobe.com/AS3/2006/builtin")])
         */

        return false;
    }
}