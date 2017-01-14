using System;
using System.Collections.Generic;
using System.IO;
using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ABCFile : FlashItem, IDisposable
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
                return "Version: " + Version;
            }
        }

        public ABCFile()
        {
            Methods = new List<ASMethod>();
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
            PopulateList(Metadata, ReadMetadata);
            PopulateList(Instances, ReadInstance);
            PopulateList(Classes, ReadClass, Instances.Count);
            PopulateList(Scripts, ReadScript);
            PopulateList(MethodBodies, ReadMethodBody);
        }

        private ASMethod ReadMethod(int index)
        {
            return new ASMethod(this, _input);
        }
        private ASMetadata ReadMetadata(int index)
        {
            return new ASMetadata(this, _input);
        }
        private ASInstance ReadInstance(int index)
        {
            return new ASInstance(this, _input);
        }
        private ASClass ReadClass(int index)
        {
            var @class = new ASClass(this, _input);
            @class.InstanceIndex = index;

            return @class;
        }
        private ASScript ReadScript(int index)
        {
            return new ASScript(this, _input);
        }
        private ASMethodBody ReadMethodBody(int index)
        {
            return new ASMethodBody(this, _input);
        }

        private void PopulateList<T>(List<T> list, Func<int, T> reader, int count = -1)
        {
            list.Capacity = (count < 0 ? _input.ReadInt30() : count);
            for (int i = 0; i < list.Capacity; i++)
            {
                T value = reader(i);
                list.Add(value);
            }
        }

        public override void WriteTo(FlashWriter output)
        {
            output.Write((ushort)Version.Minor);
            output.Write((ushort)Version.Major);
            Pool.WriteTo(output);
            WriteTo(output, Methods);
            WriteTo(output, Metadata);
            WriteTo(output, Instances);
            WriteTo(output, Classes, false);
            WriteTo(output, Scripts);
            WriteTo(output, MethodBodies);
        }
        private void WriteTo<T>(FlashWriter output, List<T> list, bool writeCount = true)
            where T : FlashItem
        {
            if (writeCount)
            {
                output.WriteInt30(list.Count);
            }
            for (int i = 0; i < list.Count; i++)
            {
                FlashItem item = list[i];
                item.WriteTo(output);
            }
        }

        public byte[] ToArray()
        {
            using (var memOutput = new MemoryStream(_initialLength))
            using (var output = new FlashWriter(memOutput))
            {
                WriteTo(output);
                return memOutput.ToArray();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _input.Dispose();
            }
        }
    }
}