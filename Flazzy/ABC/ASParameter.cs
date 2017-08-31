using System.Linq;
using System.Diagnostics;

namespace Flazzy.ABC
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class ASParameter
    {
        private readonly ASMethod _method;
        private readonly ASConstantPool _pool;

        public int ValueIndex { get; set; }
        public object Value => _pool.GetConstant(ValueKind, ValueIndex);

        public int NameIndex { get; set; }
        public string Name => _pool.Strings[NameIndex];

        public int TypeIndex { get; set; }
        public ASMultiname Type => _pool.Multinames[TypeIndex];

        public bool IsOptional { get; set; }
        public ConstantKind ValueKind { get; set; }

        public ASParameter(ASConstantPool pool, ASMethod method)
        {
            _pool = pool;
            _method = method;
        }

        public override string ToString()
        {
            string name = Name;
            if (string.IsNullOrWhiteSpace(name))
            {
                name = ("param" + (_method.Parameters.IndexOf(this) + 1));
            }

            string type = (Type?.Name ?? "*");
            if (Type?.Kind == MultinameKind.TypeName)
            {
                type = (Type.QName.Name + ".<");
                type += string.Join(", ", Type.GetTypes().Select(t => (t?.Name ?? "*")));
                type += ">";
            }

            string optionalSuffix = string.Empty;
            if (IsOptional)
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

                    case ConstantKind.Null:
                    optionalSuffix += "null";
                    break;

                    default:
                    optionalSuffix += Value;
                    break;
                }
            }

            return $"{name}:{type}{optionalSuffix}";
        }
    }
}