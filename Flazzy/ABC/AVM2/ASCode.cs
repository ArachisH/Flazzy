using System;
using System.Collections.Generic;

using Flazzy.IO;
using Flazzy.ABC.AVM2.Instructions;

namespace Flazzy.ABC.AVM2
{
    public class ASCode : FlashItem
    {
        private readonly ABCFile _abc;
        private readonly ASMethodBody _body;

        public List<Instruction> Instructions { get; }

        public ASCode(ABCFile abc, ASMethodBody body)
        {
            _abc = abc;
            _body = body;

            Instructions = new List<Instruction>();

            LoadInstructions();
        }

        private void LoadInstructions()
        {
            using (var inCode = new FlashReader(_body.Code))
            {
                while (inCode.IsDataAvailable)
                {
                    var instruction = Instruction.Create(_abc, inCode);
                    Instructions.Add(instruction);
                }
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}