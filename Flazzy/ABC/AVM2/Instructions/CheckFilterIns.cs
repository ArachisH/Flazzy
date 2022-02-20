﻿namespace Flazzy.ABC.AVM2.Instructions
{
    public class CheckFilterIns : ASInstruction
    {
        public CheckFilterIns()
            : base(OPCode.CheckFilter)
        { }

        public override int GetPopCount() => 1;
        public override int GetPushCount() => 1;
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
            machine.Values.Push(null);
        }
    }
}