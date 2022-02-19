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

        public ASClass(ABCFile abc)
            : base(abc)
        { }
        public ASClass(ABCFile abc, ref FlashReader input)
            : base(abc)
        {
            ConstructorIndex = input.ReadEncodedInt();
            Constructor.IsConstructor = true;
            Constructor.Container = this;

            PopulateTraits(ref input);
        }

        public void WriteTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(ConstructorIndex);
            base.WriteTo(ref output);
        }

        public override string ToAS3() => Instance.ToAS3();
        public override string ToString() => ToAS3();
    }
}