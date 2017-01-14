using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class ConvertBIns : Instruction
    {
        public ConvertBIns()
            : base(OPCode.Convert_b)
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
                result = Convert.ToBoolean(value);
            }
            machine.Values.Push(result);
        }
    }
}