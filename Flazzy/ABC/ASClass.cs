using System;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASClass : ASContainer
    {
        public ASInstance Instance
        {
            get { return ABC.Instances[InstanceIndex]; }
        }
        internal int InstanceIndex { get; set; }

        public ASMethod Constructor
        {
            get { return ABC.Methods[ConstructorIndex]; }
        }
        public int ConstructorIndex { get; set; }

        public override ASMultiname QName
        {
            get
            {
                return Instance.QName;
            }
        }
        public override bool IsStatic => true;
        protected override string DebuggerDisplay
        {
            get
            {
                return Instance.ToAS3();
            }
        }

        public ASClass(ABCFile abc)
            : base(abc)
        { }
        public ASClass(ABCFile abc, FlashReader input)
            : base(abc)
        {
            ConstructorIndex = input.ReadInt30();
            PopulateTraits(input);
        }

        public override string ToAS3()
        {
            return Instance.ToAS3();
        }
        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(ConstructorIndex);
            base.WriteTo(output);
        }
    }
}