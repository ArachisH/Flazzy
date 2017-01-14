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

        protected void PopulateTraits(FlashReader input)
        {
            Traits.Capacity = input.ReadInt30();
            for (int i = 0; i < Traits.Capacity; i++)
            {
                var trait = new ASTrait(ABC, input);
                trait.IsStatic = IsStatic;

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