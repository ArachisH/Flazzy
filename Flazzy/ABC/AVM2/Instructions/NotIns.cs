using System;

namespace Flazzy.ABC.AVM2.Instructions
{
    public class NotIns : Instruction
    {
        public NotIns()
            : base(OPCode.Not)
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
            object value = machine.Values.Pop();
            if (value != null)
            {
                value = !Convert.ToBoolean(value);
            }
            machine.Values.Push(value);
        }
    }
}