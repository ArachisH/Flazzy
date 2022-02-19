using Flazzy.IO;
using Flazzy.ABC.AVM2;

namespace Flazzy.ABC
{
    public class ASMethodBody : ASContainer
    {
        public byte[] Code { get; set; }
        public int MaxStack { get; set; }
        public int LocalCount { get; set; }
        public int MaxScopeDepth { get; set; }
        public int InitialScopeDepth { get; set; }

        public int MethodIndex { get; set; }
        public ASMethod Method => ABC.Methods[MethodIndex];

        public List<ASException> Exceptions { get; }

        public override ASMultiname QName => Method.Trait?.QName;

        public ASMethodBody(ABCFile abc)
            : base(abc)
        {
            Exceptions = new List<ASException>();
        }
        public ASMethodBody(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            MethodIndex = input.ReadEncodedInt();
            Method.Body = this;

            MaxStack = input.ReadEncodedInt();
            LocalCount = input.ReadEncodedInt();
            InitialScopeDepth = input.ReadEncodedInt();
            MaxScopeDepth = input.ReadEncodedInt();

            Code = new byte[input.ReadEncodedInt()];
            input.ReadBytes(Code);

            Exceptions.Capacity = input.ReadEncodedInt();
            for (int i = 0; i < Exceptions.Capacity; i++)
            {
                Exceptions.Add(new ASException(abc, ref input));
            }
            PopulateTraits(ref input);
        }

        public ASCode ParseCode()
        {
            return new ASCode(ABC, this);
        }

        public override int GetSize()
        {
            int size = 0;
            size += FlashWriter.GetEncodedIntSize(MethodIndex);
            size += FlashWriter.GetEncodedIntSize(MaxStack);
            size += FlashWriter.GetEncodedIntSize(LocalCount);
            size += FlashWriter.GetEncodedIntSize(InitialScopeDepth);
            size += FlashWriter.GetEncodedIntSize(MaxScopeDepth);

            size += FlashWriter.GetEncodedIntSize(Code.Length);
            size += Code.Length;

            size += FlashWriter.GetEncodedIntSize(Exceptions.Count);
            for (int i = 0; i < Exceptions.Count; i++)
            {
                size += Exceptions[i].GetSize();
            }

            size += base.GetSize();
            return size;
        }
        public override void WriteTo(ref FlashWriter output)
        {
            output.WriteEncodedInt(MethodIndex);
            output.WriteEncodedInt(MaxStack);
            output.WriteEncodedInt(LocalCount);
            output.WriteEncodedInt(InitialScopeDepth);
            output.WriteEncodedInt(MaxScopeDepth);

            output.WriteEncodedInt(Code.Length);
            output.Write(Code);

            output.WriteEncodedInt(Exceptions.Count);
            for (int i = 0; i < Exceptions.Count; i++)
            {
                Exceptions[i].WriteTo(ref output);
            }
            base.WriteTo(ref output);
        }

        public override string ToString() => $"LocalCount: {LocalCount:n0}, MaxStack: {MaxStack:n0}";

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }
    }
}