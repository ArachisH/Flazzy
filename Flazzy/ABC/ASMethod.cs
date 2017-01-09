using System;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASMethod : AS3Item
    {
        public string Name
        {
            get { return ABC.Pool.Strings[NameIndex]; }
        }
        public int NameIndex { get; set; }

        public ASMultiname ReturnType
        {
            get { return ABC.Pool.Multinames[ReturnTypeIndex]; }
        }
        public int ReturnTypeIndex { get; set; }

        public MethodFlags Flags { get; set; }
        public List<ASParameter> Parameters { get; }

        public ASMethod(ABCFile abc)
            : base(abc)
        {
            Parameters = new List<ASParameter>();
        }
        public ASMethod(ABCFile abc, FlashReader input)
            : this(abc)
        {
            Parameters.Capacity = input.ReadInt30();
            ReturnTypeIndex = input.ReadInt30();

            for (int i = 0; i < Parameters.Capacity; i++)
            {
                var parameter = new ASParameter(abc, this);
                parameter.TypeIndex = input.ReadInt30();
                Parameters.Add(parameter);
            }

            NameIndex = input.ReadInt30();
            Flags = (MethodFlags)input.ReadByte();

            if (Flags.HasFlag(MethodFlags.HasOptional))
            {
                int optionalParamCount = input.ReadInt30();
                for (int i = (Parameters.Count - optionalParamCount);
                    optionalParamCount > 0;
                    i++, optionalParamCount--)
                {
                    ASParameter parameter = Parameters[i];
                    parameter.IsOptional = true;
                    parameter.ValueIndex = input.ReadInt30();
                    parameter.ValueKind = (ConstantKind)input.ReadByte();
                }
            }

            if (Flags.HasFlag(MethodFlags.HasParamNames))
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    ASParameter parameter = Parameters[i];
                    parameter.NameIndex = input.ReadInt30();
                }
            }
        }

        public override string ToAS3()
        {
            throw new NotImplementedException();
        }
        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}