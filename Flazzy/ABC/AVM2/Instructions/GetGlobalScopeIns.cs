namespace Flazzy.ABC.AVM2.Instructions
{
    public class GetGlobalScopeIns : ASInstruction
    {
        public GetGlobalScopeIns()
            : base(OPCode.GetGlobalScope)
        { }

        public override int GetPushCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            machine.Values.Push(null);
        }
    }
}