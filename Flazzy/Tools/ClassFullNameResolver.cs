using System.Diagnostics;

using Flazzy.ABC;

namespace Flazzy.Tools;

public class ClassFullNameResolver
{
    public void Process(ABCFile abc)
    {
        foreach (ASClass @class in abc.Classes)
        {
            if (!IsSearchingClass(@class)) continue;

            ASInstance instance = @class.Instance;
            // Do not compare based on the index of the string value in the pool, as it is possible for two equal strings to have different indices.
            if (instance.QName.Name.Equals(instance.Constructor.Name, StringComparison.OrdinalIgnoreCase)) continue;

            string resolvedNamespaceName = null;
            string resolvedQualifiedName = null;

            /* -------- Resolve by Instance Constructor -------- */
            // Check if the constructor name isn't already the same as the class name, and only check the name end of the constructor as it may have a prefix of 'package/*'.
            string constructorName = @class.Instance.Constructor.Name;
            if (!string.IsNullOrWhiteSpace(constructorName) && !constructorName.EndsWith(@class.QName.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                if (@class.Instance.Flags.HasFlag(ClassFlags.ProtectedNamespace))
                {
                    Debugger.Break();
                }

                // Check for correct namespace on a second pass?
                resolvedQualifiedName = constructorName;
            }

            // Was the real qualified name for the class resolved?
            if (string.IsNullOrWhiteSpace(resolvedQualifiedName))
            {
                /* -------- Resolve by Trait(s) -------- */
                foreach (ASTrait trait in @class.Traits.Concat(@class.Instance.Traits))
                {
                    if (ResolveByTrait(@class, trait, out resolvedNamespaceName, out resolvedQualifiedName)) break;

                    ASMethod method = trait.Method ?? trait.Function;
                    if (method == null || !IsSearchingMethod(@class, trait)) continue; // Trait has no method, or trait not desirable for searching.

                    /* -------- Resolve by Instruction(s) -------- */
                    if (ResolveByInstruction(@class, method, out resolvedNamespaceName, out resolvedQualifiedName)) break;
                }
            }

            // Failed to resolve real qualified class name, continue iterating.
            if (string.IsNullOrWhiteSpace(resolvedQualifiedName)) continue;

            @class.ABC.Pool.Strings[@class.QName.NameIndex] = resolvedQualifiedName;
            if (!string.IsNullOrWhiteSpace(resolvedNamespaceName))
            {
                @class.ABC.Pool.Strings[@class.QName.Namespace.NameIndex] = resolvedNamespaceName;
            }

            if (@class.Instance.Flags.HasFlag(ClassFlags.ProtectedNamespace))
            {
                @class.ABC.Pool.Strings[@class.Instance.ProtectedNamespace.NameIndex] = $"{@class.QName.Namespace.Name}:{resolvedQualifiedName}";
            }
        }
    }

    protected virtual bool IsSearchingClass(ASClass @class) => true;
    protected virtual bool IsSearchingMethod(ASClass @class, ASTrait trait) => false;

    protected virtual bool ResolveByTrait(ASClass @class, ASTrait trait, out string resolvedNamespaceName, out string resolvedQualifiedName)
    {
        resolvedQualifiedName = resolvedNamespaceName = null;
        if (trait.QName.Namespace.Name.StartsWith("http")) return false;

        int separatorIndex = trait.QName.Namespace.Name.IndexOf(':');
        if (separatorIndex == -1) return false;

        // Continue iterating through the list of traits until a mismatch is found, as this will usually indicate if a qualified name was scrambled.
        if (trait.QName.Namespace.Name.Equals($"{@class.QName.Namespace.Name}:{@class.QName.Name}", StringComparison.InvariantCultureIgnoreCase)) return false;

        if (!trait.QName.Namespace.Name.StartsWith(@class.QName.Namespace.Name))
        {
            resolvedNamespaceName = trait.QName.Namespace.Name.Substring(0, separatorIndex);
        }
        resolvedQualifiedName = trait.QName.Namespace.Name.Substring(separatorIndex + 1);

        return true;
    }
    protected virtual bool ResolveByInstruction(ASClass @class, ASMethod method, out string resolvedNamespaceName, out string resolvedQualifiedName)
    {
        /*
         * As a last resort, if no name has been successfully resolved, we can attempt to extract the fully qualified name from an instruction attempting to resolve the current instance/scope.
         * If an internal method like 'toString' is called on a locally scoped/initialized variable(try/catch, switch, etc), it may utilize the real fully qualified name of the class when invoking the internal method call.
         * setproperty MultinameL([ !!>>> PrivateNamespace("com.hurlant.util:Hex") <<<!! ,StaticProtectedNs("com.hurlant.util:Hex"),StaticProtectedNs("Object"),PackageNamespace("com.hurlant.util"),PackageInternalNs("com.hurlant.util"),PrivateNamespace("FilePrivateNS:Hex"),PackageNamespace(""),Namespace("http://adobe.com/AS3/2006/builtin")])
         */

        resolvedNamespaceName = resolvedQualifiedName = null;
        return false;
    }
}