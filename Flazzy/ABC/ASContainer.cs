using System.Linq;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public abstract class ASContainer : AS3Item
    {
        public List<ASTrait> Traits { get; }

        public virtual bool IsStatic { get; }
        public abstract ASMultiname QName { get; }
        protected override string DebuggerDisplay
        {
            get
            {
                int methodCount = Traits.Count(
                    t => t.Kind == TraitKind.Method ||
                         t.Kind == TraitKind.Getter ||
                         t.Kind == TraitKind.Setter);

                int slotCount = Traits.Count(t => t.Kind == TraitKind.Slot);
                int constantCount = Traits.Count(t => t.Kind == TraitKind.Constant);

                return $"{QName}, Traits: {Traits.Count}";
            }
        }

        public ASContainer(ABCFile abc)
            : base(abc)
        {
            Traits = new List<ASTrait>();
        }

        public void AddMethod(ASMethod method, string qualifiedName)
        {
            var qname = new ASMultiname(ABC.Pool);
            qname.NameIndex = ABC.Pool.AddConstant(qualifiedName);
            qname.Kind = MultinameKind.QName;
            qname.NamespaceIndex = 1; // Public

            int methodIndex = ABC.AddMethod(method);
            int qnameIndex = ABC.Pool.AddConstant(qname);

            var trait = new ASTrait(ABC);
            trait.Kind = TraitKind.Method;
            trait.QNameIndex = qnameIndex;
            trait.MethodIndex = methodIndex;

            method.Trait = trait;
            method.Container = this;
            Traits.Add(trait);
        }

        public IEnumerable<ASMethod> GetMethods()
        {
            return GetTraits(TraitKind.Method, TraitKind.Getter, TraitKind.Setter).Select(t => t.Method);
        }
        public IEnumerable<ASMethod> GetMethods(string qualifiedName) => GetMethods(qualifiedName, null, null);
        public IEnumerable<ASMethod> GetMethods(string qualifiedName, string returnTypeName) => GetMethods(qualifiedName, returnTypeName, null);
        public IEnumerable<ASMethod> GetMethods(string qualifiedName, string returnTypeName, int paramCount) => GetMethods(qualifiedName, returnTypeName, new string[paramCount]);
        public IEnumerable<ASMethod> GetMethods(string qualifiedName, string returnTypeName, string[] paramTypeNames)
        {
            foreach (ASMethod method in GetMethods())
            {
                if (qualifiedName != null && method.Trait.QName.Name != qualifiedName) continue;
                if (returnTypeName != null && method.ReturnType.Name != returnTypeName) continue;
                if (paramTypeNames != null)
                {
                    if (method.Parameters.Count != paramTypeNames.Length) continue;

                    bool isContinuing = false;
                    for (int i = 0; i < paramTypeNames.Length; i++)
                    {
                        if (paramTypeNames[i] == null) continue;
                        if (method.Parameters[i].Type.Name != paramTypeNames[i])
                        {
                            isContinuing = true;
                            break;
                        }
                    }
                    if (isContinuing) continue;
                }
                yield return method;
            }
        }

        public ASMethod GetMethod(string qualifiedName) => GetMethods(qualifiedName).FirstOrDefault();
        public ASMethod GetMethod(string qualifiedName, string returnTypeName) => GetMethods(qualifiedName, returnTypeName).FirstOrDefault();
        public ASMethod GetMethod(string qualifiedName, string returnTypeName, int paramCount) => GetMethods(qualifiedName, returnTypeName, paramCount).FirstOrDefault();
        public ASMethod GetMethod(string qualifiedName, string returnTypeName, string[] paramTypeNames) => GetMethods(qualifiedName, returnTypeName, paramTypeNames).FirstOrDefault();

        public IEnumerable<ASTrait> GetGetters()
        {
            return GetTraits(TraitKind.Getter);
        }
        public IEnumerable<ASTrait> GetGetters(string returnTypeName)
        {
            return GetGetters()
                .Where(g => g.Type.Name == returnTypeName);
        }

        public ASTrait GetGetter(string qualifiedName)
        {
            return GetGetters()
                .FirstOrDefault(g => g.QName.Name == qualifiedName);
        }

        public IEnumerable<ASTrait> GetSlotTraits(string returnTypeName)
        {
            return GetTraits(TraitKind.Slot)
                .Where(sct => (sct.Type?.Name ?? "*") == returnTypeName);
        }
        public IEnumerable<ASTrait> GetConstantTraits(string returnTypeName)
        {
            return GetTraits(TraitKind.Constant)
                .Where(sct => (sct.Type?.Name ?? "*") == returnTypeName);
        }

        public ASTrait GetSlot(string qualifiedName)
        {
            return GetTraits(TraitKind.Slot).Single(st => st.QName.Name == qualifiedName);
        }
        public ASTrait GetConstant(string qualifiedName)
        {
            return GetTraits(TraitKind.Constant).Single(ct => ct.QName.Name == qualifiedName);
        }

        public IEnumerable<ASTrait> GetTraits(params TraitKind[] kinds)
        {
            if ((kinds?.Length ?? 0) == 0) Enumerable.Empty<ASTrait>();
            return Traits.Where(t => kinds.Contains(t.Kind));
        }

        protected void PopulateTraits(FlashReader input)
        {
            Traits.Capacity = input.ReadInt30();
            for (int i = 0; i < Traits.Capacity; i++)
            {
                var trait = new ASTrait(ABC, input);
                trait.IsStatic = IsStatic;

                if (trait.Kind == TraitKind.Method ||
                    trait.Kind == TraitKind.Getter ||
                    trait.Kind == TraitKind.Setter)
                {
                    trait.Method.Container = this;
                }

                Traits.Add(trait);
            }
        }
        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(Traits.Count);
            for (int i = 0; i < Traits.Count; i++)
            {
                ASTrait trait = Traits[i];
                trait.WriteTo(output);
            }
        }
    }
}