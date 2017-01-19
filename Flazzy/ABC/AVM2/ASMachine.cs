using System.Collections.Generic;

namespace Flazzy.ABC.AVM2
{
    public class ASMachine
    {
        private readonly ASCode _code;

        public Stack<object> Values { get; }
        public Stack<object> Scopes { get; }
        public Dictionary<int, object> Registers { get; }

        public ASMachine(ASCode code, int localCount)
        {
            _code = code;

            Values = new Stack<object>();
            Scopes = new Stack<object>();

            Registers = new Dictionary<int, object>();
            for (int i = 0; i < localCount; i++)
            {
                Registers[i] = null;
            }
        }
    }
}