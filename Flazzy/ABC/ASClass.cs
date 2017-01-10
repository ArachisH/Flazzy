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
            throw new NotImplementedException();
        }
        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}