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
            MethodIndex = input.ReadInt30();
            Method.Body = this;

            MaxStack = input.ReadInt30();
            LocalCount = input.ReadInt30();
            InitialScopeDepth = input.ReadInt30();
            MaxScopeDepth = input.ReadInt30();

            Code = new byte[input.ReadInt30()];
            input.ReadBytes(Code);

            Exceptions.Capacity = input.ReadInt30();
            for (int i = 0; i < Exceptions.Capacity; i++)
            {
                var exception = new ASException(abc, ref input);
                Exceptions.Add(exception);
            }
            PopulateTraits(ref input);
        }

        public ASCode ParseCode()
        {
            return new ASCode(ABC, this);
        }

        public override int GetSize() => throw new NotImplementedException();
        public override void WriteTo(FlashWriter output)
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
                ASException exception = Exceptions[i];
                exception.WriteTo(output);
            }
            base.WriteTo(output);
        }

        public override string ToString() => $"LocalCount: {LocalCount:n0}, MaxStack: {MaxStack:n0}";

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }
    }
}