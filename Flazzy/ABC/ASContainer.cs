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

        public IEnumerable<ASMethod> GetMethods()
        {
            return GetTraits(TraitKind.Method,
                TraitKind.Getter, TraitKind.Setter)
                .Select(t => t.Method);
        }
        public IEnumerable<ASMethod> GetMethods(int paramCount)
        {
            return GetMethods()
                .Where(m => m.Parameters.Count == paramCount);
        }
        public IEnumerable<ASMethod> GetMethods(string returnTypeName)
        {
            return GetMethods()
                .Where(m => m.ReturnType.Name == returnTypeName);
        }
        public IEnumerable<ASMethod> GetMethods(int paramCount, string returnTypeName)
        {
            return GetMethods()
                .Where(m => m.Parameters.Count == paramCount &&
                            m.ReturnType.Name == returnTypeName);
        }
        public ASMethod GetMethod(int paramCount, string returnTypeName, string methodName)
        {
            return GetMethods()
                .Where(m => m.Trait.QName.Name == methodName &&
                            m.Parameters.Count == paramCount &&
                            m.ReturnType.Name == returnTypeName)
                .FirstOrDefault();
        }

        public IEnumerable<ASTrait> GetSlotTraits(string typeName)
        {
            return GetTraits(TraitKind.Slot)
                .Where(sct => sct.Type.Name == typeName);
        }
        public IEnumerable<ASTrait> GetConstantTraits(string typeName)
        {
            return GetTraits(TraitKind.Constant)
                .Where(sct => sct.Type.Name == typeName);
        }

        public IEnumerable<ASTrait> GetTraits(params TraitKind[] kinds)
        {
            if (kinds.Length > 0)
            {
                foreach (ASTrait trait in Traits)
                {
                    if (kinds.Contains(trait.Kind))
                        yield return trait;
                }
            }
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