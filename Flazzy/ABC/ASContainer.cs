using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public abstract class ASContainer : AS3Item
    {
        public List<ASTrait> Traits { get; }

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
                Traits.Add(trait);
            }
        }
    }
}