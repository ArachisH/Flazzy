using System.Collections.Generic;

namespace Flazzy.ABC.AVM2
{
    public class ASMachine
    {
        private readonly ASCode _code;

        public Stack<object> Values { get; }
        public Stack<object> Scopes { get; }
        public Dictionary<int, object> Registers { get; }

        public ASMachine(ASCode code)
        {
            _code = code;

            Values = new Stack<object>();
            Scopes = new Stack<object>();
            Registers = new Dictionary<int, object>();
        }
    }
}