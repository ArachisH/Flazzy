namespace Flazzy.ABC.AVM2.Instructions
{
    public class PushScopeIns : ASInstruction
    {
        public PushScopeIns()
            : base(OPCode.PushScope)
        { }

        public override int GetPopCount()
        {
            return 1;
        }
        public override void Execute(ASMachine machine)
        {
            object value = machine.Values.Pop();
            machine.Scopes.Push(value);
        }
    }
}