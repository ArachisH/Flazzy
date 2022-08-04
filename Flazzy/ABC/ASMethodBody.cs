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
        protected override string DebuggerDisplay => $"LocalCount: {LocalCount:n0}, MaxStack: {MaxStack:n0}";

        public ASMethodBody(ABCFile abc)
            : base(abc)
        {
            Exceptions = new List<ASException>();
        }
        public ASMethodBody(ABCFile abc, FlashReader input)
            : this(abc)
        {
            MethodIndex = input.ReadInt30();
            Method.Body = this;

            MaxStack = input.ReadInt30();
            LocalCount = input.ReadInt30();
            InitialScopeDepth = input.ReadInt30();
            MaxScopeDepth = input.ReadInt30();

            int codeLength = input.ReadInt30();
            Code = input.ReadBytes(codeLength);

            Exceptions.Capacity = input.ReadInt30();
            for (int i = 0; i < Exceptions.Capacity; i++)
            {
                var exception = new ASException(abc, input);
                Exceptions.Add(exception);
            }
            PopulateTraits(input);
        }

        public ASCode ParseCode()
        {
            return new ASCode(ABC, this);
        }
        
        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(MethodIndex);
            output.WriteInt30(MaxStack);
            output.WriteInt30(LocalCount);
            output.WriteInt30(InitialScopeDepth);
            output.WriteInt30(MaxScopeDepth);

            output.WriteInt30(Code.Length);
            output.Write(Code);

            output.WriteInt30(Exceptions.Count);
            for (int i = 0; i < Exceptions.Count; i++)
            {
                ASException exception = Exceptions[i];
                exception.WriteTo(output);
            }
            base.WriteTo(output);
        }
    }
}