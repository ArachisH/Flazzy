using System;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASScript : ASContainer
    {
        public ASMethod Initializer
        {
            get { return ABC.Methods[InitializerIndex]; }
        }
        public int InitializerIndex { get; set; }

        public override ASMultiname QName
        {
            get
            {
                return Traits[0].QName;
            }
        }

        public ASScript(ABCFile abc)
            : base(abc)
        { }
        public ASScript(ABCFile abc, FlashReader input)
            : base(abc)
        {
            InitializerIndex = input.ReadInt30();
            PopulateTraits(input);
        }

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }
        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(InitializerIndex);
            base.WriteTo(output);
        }
    }
}