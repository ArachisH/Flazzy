using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ConvertUIns : Instruction
    {
        public ConvertUIns()
            : base(OPCode.Convert_u)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object result = null;
            object value = machine.Values.Pop();
            if (value != null)
            {
                result = Convert.ToUInt32(value);
            }
            machine.Values.Push(result);
        }
    }
}