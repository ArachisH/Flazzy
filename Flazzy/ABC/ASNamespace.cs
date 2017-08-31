using System;

using Flazzy.IO;

namespace Flazzy.ABC
{
    /// <summary>
    /// Represents a namespace in the bytecode.
    /// </summary>
    public class ASNamespace : ConstantItem
    {
        /// <summary>
        /// Gets or sets the index of the string in <see cref="ASConstantPool.Strings"/> representing the namespace name.
        /// </summary>
        public int NameIndex { get; set; }
        /// <summary>
        /// Gets the name of the namespace.
        /// </summary>
        public string Name => Pool.Strings[NameIndex];

        /// <summary>
        /// Gets or sets the kind of namespace this entry should be interpreted as by the loader.
        /// </summary>
        public NamespaceKind Kind { get; set; }

        protected override string DebuggerDisplay => $"{Kind}: \"{Name}\"";

        public ASNamespace(ASConstantPool pool)
            : base(pool)
        { }
        public ASNamespace(ASConstantPool pool, FlashReader input)
            : base(pool)
        {
            Kind = (NamespaceKind)input.ReadByte();
            if (!Enum.IsDefined(typeof(NamespaceKind), Kind))
            {
                throw new InvalidCastException($"Invalid namespace kind for value {Kind:0x00}.");
            }
            NameIndex = input.ReadInt30();
        }

        public string GetAS3Modifiers()
        {
            switch (Kind)
            {
                case NamespaceKind.Package: return "public";
                case NamespaceKind.Private: return "private";
                case NamespaceKind.Explicit: return "explicit";

                case NamespaceKind.StaticProtected:
                case NamespaceKind.Protected: return "protected";


                case NamespaceKind.Namespace:
                case NamespaceKind.PackageInternal:
                default: return string.Empty;
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            output.Write((byte)Kind);
            output.WriteInt30(NameIndex);
        }
    }
}