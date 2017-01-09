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
            output.WriteInt30(Parameters.Count);
            output.WriteInt30(ReturnTypeIndex);

            int optionalParamCount = 0;
            int optionalParamStartIndex = (Parameters.Count - 1);
            if (Parameters.Count > 0)
            {
                // This flag will be removed if at least a single parameter has no name assigned.
                Flags |= MethodFlags.HasParamNames;
                for (int i = 0; i < Parameters.Count; i++)
                {
                    ASParameter parameter = Parameters[i];
                    output.WriteInt30(parameter.TypeIndex);

                    // This flag should only be present when all parameters are assigned a Name.
                    if (string.IsNullOrWhiteSpace(parameter.Name))
                        Flags &= ~MethodFlags.HasParamNames;

                    // Just one optional parameter is enough to attain this flag.
                    if (parameter.IsOptional)
                    {
                        if (i < optionalParamStartIndex)
                            optionalParamStartIndex = i;

                        optionalParamCount++;
                        Flags |= MethodFlags.HasOptional;
                    }
                }
            }

            output.WriteInt30(NameIndex);
            output.Write((byte)Flags);
            if (Flags.HasFlag(MethodFlags.HasOptional))
            {
                output.WriteInt30(optionalParamCount);
                for (int i = optionalParamStartIndex; i < Parameters.Count; i++)
                {
                    ASParameter parameter = Parameters[i];
                    output.WriteInt30(parameter.ValueIndex);
                    output.Write((byte)parameter.ValueKind);
                }
            }

            if (Flags.HasFlag(MethodFlags.HasParamNames))
            {
                for (int i = 0; i < Parameters.Count; i++)
                {
                    ASParameter parameter = Parameters[i];
                    output.WriteInt30(parameter.NameIndex);
                }
            }
        }
    }
}