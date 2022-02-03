using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASScript : ASContainer // TODO: Check QName usages
    {
        public int InitializerIndex { get; set; }
        public ASMethod Initializer => ABC.Methods[InitializerIndex];

        public override ASMultiname QName => Traits[0].QName;

        public ASScript(ABCFile abc)
            : base(abc)
        { }
        public ASScript(ABCFile abc, ref FlashReader input)
            : base(abc)
        {
            InitializerIndex = input.ReadInt30();
            PopulateTraits(ref input);
        }

        public void WriteTo(FlashWriter output)
        {
            output.WriteEncodedInt(InitializerIndex);
            base.WriteTo(output);
        }

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }
    }
}