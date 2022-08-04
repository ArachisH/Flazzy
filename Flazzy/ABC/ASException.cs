using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASException : AS3Item
    {
        public int To { get; set; }
        public int From { get; set; }
        public int Target { get; set; }

        public int VariableNameIndex { get; set; }
        public ASMultiname VariableName => ABC.Pool.Multinames[VariableNameIndex];

        public int ExceptionTypeIndex { get; set; }
        public ASMultiname ExceptionType => ABC.Pool.Multinames[ExceptionTypeIndex];

        public ASException(ABCFile abc)
            : base(abc)
        { }
        public ASException(ABCFile abc, FlashReader input)
            : base(abc)
        {
            From = input.ReadInt30();
            To = input.ReadInt30();
            Target = input.ReadInt30();
            ExceptionTypeIndex = input.ReadInt30();
            VariableNameIndex = input.ReadInt30();
        }
        
        public override void WriteTo(FlashWriter output)
        {
            output.WriteInt30(From);
            output.WriteInt30(To);
            output.WriteInt30(Target);
            output.WriteInt30(ExceptionTypeIndex);
            output.WriteInt30(VariableNameIndex);
        }
    }
}