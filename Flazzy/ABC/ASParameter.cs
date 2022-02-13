using System.Text;
using System.Diagnostics;

namespace Flazzy.ABC
{
    [DebuggerDisplay("{ToString(),nq}")]
    public class ASParameter
    {
        private readonly ASMethod _method;

        public int ValueIndex { get; set; }
        public object Value => _method.ABC.Pool.GetConstant(ValueKind, ValueIndex);

        public int NameIndex { get; set; }
        public string Name => _method.ABC.Pool.Strings[NameIndex];

        public int TypeIndex { get; set; }
        public ASMultiname Type => _method.ABC.Pool.Multinames[TypeIndex];

        public bool IsOptional { get; set; }
        public ConstantKind ValueKind { get; set; }

        public ASParameter(ASMethod method)
        {
            _method = method;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            Append(builder);
            return builder.ToString();
        }
        internal void Append(StringBuilder builder, int? index = null)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                builder.Append("param");
                builder.Append(index ?? _method.Parameters.IndexOf(this) + 1);
            }
            else builder.Append(Name);

            builder.Append(':'); // Separate the parameter name, and its type.
            if (Type?.Kind == MultinameKind.TypeName)
            {
                builder.Append(Type.QName.Name);
                builder.Append(".<");

                foreach (ASMultiname multiname in Type.GetTypes())
                {
                    builder.Append(multiname?.Name ?? "*");
                    builder.Append(", ");
                }
                builder.Length -= 2; // Ignore the last two characters, in this case being ", ".
                builder.Append('>');
            }
            else builder.Append(Type?.Name ?? "*");

            if (IsOptional)
            {
                builder.Append(" = ");
                switch (ValueKind)
                {
                    case ConstantKind.String:
                    builder.Append('"');
                    builder.Append(Value);
                    builder.Append('"');
                    break;

                    case ConstantKind.Null:
                    builder.Append("null");
                    break;

                    case ConstantKind.True:
                    builder.Append("true");
                    break;

                    case ConstantKind.False:
                    builder.Append("false");
                    break;

                    default: builder.Append(Value); break;
                }
            }
        }
    }
}