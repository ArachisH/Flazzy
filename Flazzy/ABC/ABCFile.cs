using System;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ABCFile : FlashItem
    {
        private FlashReader _input;
        private readonly int _initialLength;

        public List<ASMethod> Methods { get; }
        public List<ASMetadata> Metadata { get; }
        public List<ASInstance> Instances { get; }
        public List<ASClass> Classes { get; }
        public List<ASScript> Scripts { get; }
        public List<ASMethodBody> MethodBodies { get; }

        public ASConstantPool Pool { get; }
        public Version Version { get; set; }

        protected override string DebuggerDisplay
        {
            get
            {
                return $"Version: {Version}";
            }
        }

        public ABCFile()
        {
            Methods = new List<ABC.ASMethod>();
            Metadata = new List<ASMetadata>();
            Instances = new List<ASInstance>();
            Classes = new List<ASClass>();
            Scripts = new List<ASScript>();
            MethodBodies = new List<ASMethodBody>();
        }
        public ABCFile(byte[] data)
            : this()
        {
            _initialLength = data.Length;
            _input = new FlashReader(data);

            ushort minor = _input.ReadUInt16();
            ushort major = _input.ReadUInt16();
            Version = new Version(major, minor);
            Pool = new ASConstantPool(this, _input);

            PopulateList(Methods, ReadMethod);
            //PopulateList(Metadata, ReadMetadata);
            //PopulateList(Instances, ReadInstance);
            //PopulateList(Classes, ReadClass);
            //PopulateList(Scripts, ReadScript);
            //PopulateList(MethodBodies, ReadMethodBody);
        }

        public ASMethod ReadMethod()
        {
            return new ASMethod(this, _input);
        }
        public ASMetadata ReadMetadata()
        {
            return null;
        }
        public ASInstance ReadInstance()
        {
            return null;
        }
        public ASClass ReadClass()
        {
            return null;
        }
        public ASScript ReadScript()
        {
            return null;
        }
        public ASMethodBody ReadMethodBody()
        {
            return null;
        }

        private void PopulateList<T>(List<T> list, Func<T> reader, int count = -1)
        {
            list.Capacity = (count < 0 ? _input.ReadInt30() : count);
            for (int i = 0; i < list.Capacity; i++)
            {
                T value = reader();
                list.Add(value);
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            output.Write((ushort)Version.Minor);
            output.Write((ushort)Version.Major);
            Pool.WriteTo(output);
        }
    }
}