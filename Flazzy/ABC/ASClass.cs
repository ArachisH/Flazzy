using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASClass : ASContainer
    {
        internal int InstanceIndex { get; set; }
        public ASInstance Instance => ABC.Instances[InstanceIndex];

        public int ConstructorIndex { get; set; }
        public ASMethod Constructor => ABC.Methods[ConstructorIndex];

        public override bool IsStatic => true;
        public override ASMultiname QName => Instance.QName;
        protected override string DebuggerDisplay => ToAS3();

        public ASClass(ABCFile abc)
            : base(abc)
        { }
        public ASClass(ABCFile abc, FlashReader input)
            : base(abc)
        {
            ConstructorIndex = input.ReadInt30();
            Constructor.IsConstructor = true;
            Constructor.Container = this;

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