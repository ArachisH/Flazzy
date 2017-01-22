using System;
using System.Linq;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ASParameter : AS3Item
    {
        private readonly ASMethod _method;

        public object Value
        {
            get { return ABC.Pool.GetConstant(ValueKind, ValueIndex); }
        }
        public int ValueIndex { get; set; }

        public string Name
        {
            get { return ABC.Pool.Strings[NameIndex]; }
        }
        public int NameIndex { get; set; }

        public ASMultiname Type
        {
            get { return ABC.Pool.Multinames[TypeIndex]; }
        }
        public int TypeIndex { get; set; }

        public bool IsOptional { get; set; }
        public ConstantKind ValueKind { get; set; }

        protected override string DebuggerDisplay
        {
            get
            {
                return ToAS3();
            }
        }

        public ASParameter(ABCFile abc, ASMethod method)
            : base(abc)
        {
            _method = method;
        }

        public override string ToAS3()
        {
            string name = Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = ("param" + (_method.Parameters.IndexOf(this) + 1));
            }

            string type = (Type?.Name ?? "*");
            if (Type?.Kind == MultinameKind.TypeName)
            {
                type = (Type.QName.Name + "<");
                type += string.Join(", ", Type.GetTypes().Select(t => t.Name));
                type += ">";
            }

            string optionalSuffix = string.Empty;
            if (Value != null)
            {
                optionalSuffix += " = ";
                switch (ValueKind)
                {
                    case ConstantKind.String:
                    optionalSuffix += $"\"{Value}\"";
                    break;

                    case ConstantKind.True:
                    case ConstantKind.False:
                    optionalSuffix += Value.ToString().ToLower();
                    break;

                    default:
                    optionalSuffix += Value;
                    break;
                }
            }

            return $"{name}:{type}{optionalSuffix}";
        }
        public override void WriteTo(FlashWriter output)
        {
            throw new NotImplementedException();
        }
    }
}