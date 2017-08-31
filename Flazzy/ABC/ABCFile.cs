using System;
using System.Linq;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ABCFile : FlashItem, IDisposable
    {
        private readonly FlashReader _input;
        private readonly Dictionary<string, List<ASClass>> _classCache;

        public List<ASMethod> Methods { get; }
        public List<ASMetadata> Metadata { get; }
        public List<ASInstance> Instances { get; }
        public List<ASClass> Classes { get; }
        public List<ASScript> Scripts { get; }
        public List<ASMethodBody> MethodBodies { get; }

        public ASConstantPool Pool { get; }
        public Version Version { get; set; }

        protected override string DebuggerDisplay => ("Version: " + Version);

        public ABCFile()
        {
            _classCache = new Dictionary<string, List<ASClass>>();

            Methods = new List<ASMethod>();
            Metadata = new List<ASMetadata>();
            Instances = new List<ASInstance>();
            Classes = new List<ASClass>();
            Scripts = new List<ASScript>();
            MethodBodies = new List<ASMethodBody>();
        }
        public ABCFile(byte[] data)
            : this(new FlashReader(data))
        { }
        public ABCFile(FlashReader input)
            : this()
        {
            _input = input;

            ushort minor = input.ReadUInt16();
            ushort major = input.ReadUInt16();
            Version = new Version(major, minor);
            Pool = new ASConstantPool(this, input);

            PopulateList(Methods, ReadMethod);
            PopulateList(Metadata, ReadMetadata);
            PopulateList(Instances, ReadInstance);
            PopulateList(Classes, ReadClass, Instances.Count);
            PopulateList(Scripts, ReadScript);
            PopulateList(MethodBodies, ReadMethodBody);
        }

        public int AddMethod(ASMethod method, bool recycle = true)
        {
            return AddValue(Methods, method, recycle);
        }
        public int AddMetadata(ASMetadata metadata, bool recycle = true)
        {
            return AddValue(Metadata, metadata, recycle);
        }
        public int AddClass(ASClass @class, ASInstance instance, bool recycle = true)
        {
            AddValue(Classes, @class, recycle);
            return AddValue(Instances, instance, recycle);
        }
        public int AddScript(ASScript script, bool recycle = true)
        {
            return AddValue(Scripts, script, recycle);
        }
        public int AddMethodBody(ASMethodBody methodBody, bool recycle = true)
        {
            if (methodBody.Method != null)
            {
                methodBody.Method.Body = methodBody;
            }
            return AddValue(MethodBodies, methodBody, recycle);
        }
        protected virtual int AddValue<T>(List<T> valueList, T value, bool recycle)
        {
            int index = (recycle ?
                valueList.IndexOf(value) : -1);

            if (index == -1)
            {
                index = (valueList.Count);
                valueList.Add(value);
            }
            return index;
        }

        public void RebuildCache()
        {
            _classCache.Clear();
            foreach (ASClass @class in Classes)
            {
                List<ASClass> classes = null;
                string qualifiedName = @class.Instance.QName.Name;
                if (!_classCache.TryGetValue(qualifiedName, out classes))
                {
                    classes = new List<ASClass>();
                    _classCache[qualifiedName] = classes;
                }
                classes.Add(@class);
            }
        }

        public ASClass GetFirstClass(string qualifiedName)
        {
            return GetClasses(qualifiedName).FirstOrDefault();
        }
        public IEnumerable<ASClass> GetClasses(string qualifiedName)
        {
            List<ASClass> classes = null;
            if (_classCache.TryGetValue(qualifiedName, out classes))
            {
                foreach (ASClass @class in classes)
                {
                    if (@class.Instance.QName.Name != qualifiedName) continue;
                    yield return @class;
                }
            }
        }

        public ASInstance GetFirstInstance(string qualifiedName)
        {
            return GetInstances(qualifiedName).FirstOrDefault();
        }
        public IEnumerable<ASInstance> GetInstances(string qualifiedName)
        {
            foreach (ASClass @class in GetClasses(qualifiedName))
            {
                yield return @class.Instance;
            }
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

            List<ASClass> classes = null;
            string qualifedName = @class.Instance.QName.Name;
            if (!_classCache.TryGetValue(qualifedName, out classes))
            {
                classes = new List<ASClass>();
                _classCache[qualifedName] = classes;
            }

            classes.Add(@class);
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