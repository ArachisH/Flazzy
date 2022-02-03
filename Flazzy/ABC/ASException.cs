using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASException : IAS3Item
    {
        public ABCFile ABC { get; }

        public int To { get; set; }
        public int From { get; set; }
        public int Target { get; set; }

        public int VariableNameIndex { get; set; }
        public ASMultiname VariableName => ABC.Pool.Multinames[VariableNameIndex];

        public int ExceptionTypeIndex { get; set; }
        public ASMultiname ExceptionType => ABC.Pool.Multinames[ExceptionTypeIndex];

        public ASException(ABCFile abc)
        {
            ABC = abc;
        }
        public ASException(ABCFile abc, ref FlashReader input)
            : this(abc)
        {
            From = input.ReadInt30();
            To = input.ReadInt30();
            Target = input.ReadInt30();
            ExceptionTypeIndex = input.ReadInt30();
            VariableNameIndex = input.ReadInt30();
        }
        
        public int GetSize()
        {
            throw new NotImplementedException();
        }
        public void WriteTo(FlashWriter output)
        {
            output.WriteEncodedInt(From);
            output.WriteEncodedInt(To);
            output.WriteEncodedInt(Target);
            output.WriteEncodedInt(ExceptionTypeIndex);
            output.WriteEncodedInt(VariableNameIndex);
        }

        public string ToAS3()
        {
            throw new NotImplementedException();
        }
    }
}