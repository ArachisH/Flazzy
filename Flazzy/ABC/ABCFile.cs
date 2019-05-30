using System;
using System.Linq;
using System.Collections.Generic;

using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ABCFile : FlashItem, IDisposable
    {
        private readonly FlashReader _input;
        private readonly Dictionary<ASMultiname, List<ASClass>> _classesCache;

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
            _classesCache = new Dictionary<ASMultiname, List<ASClass>>();

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

        public ASClass GetClass(ASMultiname multiname) => GetClasses(multiname).FirstOrDefault();
        public ASClass GetClass(string qualifiedName) => GetClass(GetMultiname(qualifiedName));

        public ASInstance GetInstance(ASMultiname multiname) => GetInstances(multiname).FirstOrDefault();
        public ASInstance GetInstance(string qualifiedName) => GetInstance(GetMultiname(qualifiedName));

        public IEnumerable<ASClass> GetClasses(ASMultiname multiname)
        {
            if (multiname == null || !_classesCache.TryGetValue(multiname, out List<ASClass> classes)) yield break;
            for (int i = 0; i < classes.Count; i++)
            {
                ASClass @class = classes[i];
                if (@class.QName != multiname)
                {
                    i--;
                    classes.RemoveAt(i);
                    if (!_classesCache.TryGetValue(@class.QName, out List<ASClass> newClasses))
                    {
                        newClasses = new List<ASClass>();
                        _classesCache.Add(@class.QName, newClasses);
                    }
                    newClasses.Add(@class);
                }
                else yield return @class;
            }
        }
        public IEnumerable<ASClass> GetClasses(string qualifiedName) => GetClasses(GetMultiname(qualifiedName));

        public IEnumerable<ASInstance> GetInstances(ASMultiname multiname) => GetClasses(multiname).Select(c => c.Instance);
        public IEnumerable<ASInstance> GetInstances(string qualifiedName) => GetInstances(GetMultiname(qualifiedName));

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

            if (!_classesCache.TryGetValue(@class.QName, out List<ASClass> classes))
            {
                classes = new List<ASClass>();
                _classesCache.Add(@class.QName, classes);
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

        private ASMultiname GetMultiname(string qualifiedName)
        {
            foreach (ASMultiname multiname in Pool.GetMultinames(qualifiedName))
            {
                if (multiname.Kind == MultinameKind.QName) return multiname;
            }
            return null;
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
                _classesCache.Clear();

                Methods.Clear();
                Metadata.Clear();
                Instances.Clear();
                Classes.Clear();
                Scripts.Clear();
                MethodBodies.Clear();

                Pool.Integers.Clear();
                Pool.UIntegers.Clear();
                Pool.Doubles.Clear();
                Pool.Strings.Clear();
                Pool.Namespaces.Clear();
                Pool.NamespaceSets.Clear();
                Pool.Multinames.Clear();
            }
        }
    }
}