using Flazzy.ABC;
using Flazzy.ABC.AVM2;
using Flazzy.ABC.AVM2.Instructions;

namespace Flazzy.Tools;

public sealed class StringObfuscation
{
    public static bool TryDeobfuscate(ASClass @class, string methodQualifiedName)
    {
        int multinameIndex = 0;
        // Only static class contains the 'getKeyValue' method.
        foreach (ASTrait trait in @class.Traits.Where(t => t.Kind == TraitKind.Method))
        {
            if (!trait.QName.Name.Equals(methodQualifiedName, StringComparison.OrdinalIgnoreCase)) continue;

            if (trait.Kind != TraitKind.Method) continue;
            if (trait.Method.Parameters.Count != 2) continue;

            if (trait.Method.Parameters[0].Type.Name != "Array") continue;
            if (trait.Method.Parameters[1].Type.Name != "int") continue;

            multinameIndex = trait.QNameIndex;
        }
        if (multinameIndex == 0) return false;

        foreach (ASTrait trait in @class.Traits.Concat(@class.Instance.Traits).Where(t => t.Kind == TraitKind.Method))
        {
            ASMethod method = trait.Method ?? trait.Function;
            if (method == null) continue;

            ASCode code = method.Body.ParseCode();
            code.Deobfuscate();

            for (int i = 0; i < code.Count; i++)
            {
                ASInstruction instruction = code[i];
                if (instruction.OP != OPCode.FindPropStrict) continue;

                var findPropStrictIns = (FindPropStrictIns)instruction;
                if (findPropStrictIns.PropertyNameIndex != multinameIndex) continue;

                // Read
            }
        }

        return false;
    }
}