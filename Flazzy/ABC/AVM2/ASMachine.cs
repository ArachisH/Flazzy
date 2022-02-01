namespace Flazzy.ABC.AVM2
{
    public class ASMachine
    {
        public Stack<object> Values { get; }
        public Stack<object> Scopes { get; }
        public Dictionary<int, object> Registers { get; }

        public ASMachine(int localCount)
        {
            Values = new Stack<object>();
            Scopes = new Stack<object>();
            Registers = new Dictionary<int, object>(localCount);
        }
    }
}