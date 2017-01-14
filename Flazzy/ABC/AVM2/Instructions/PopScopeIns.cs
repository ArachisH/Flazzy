namespace Flazzy.ABC.AVM2.Instructions
{
    public class PopScopeIns : Instruction
    {
        public PopScopeIns()
            : base(OPCode.PopScope)
        { }

        public override void Execute(ASMachine machine)
        {
            machine.Scopes.Pop();
        }
    }
}