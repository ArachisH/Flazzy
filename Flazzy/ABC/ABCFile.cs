using Flazzy.IO;

namespace Flazzy.ABC
{
    public class ABCFile : FlashItem, IDisposable
    {
        private readonly FlashReader _input;
        private readonly Dictionary<ASMultiname, List<ASClass>> _classByQNameCache;
        private readonly Dictionary<string, ASInstance> _instanceByConstructorCache;

        public List<ASMethod> Methods { get; }
        public List<ASMetadata> Metadata { get; }
        public List<ASInstance> Instances { get; }
        public List<ASClass> Classes { get; }
        public List<ASScript> Scripts { get; }
        public List<ASMethodBody> MethodBodies { get; }

        public ASConstantPool Pool { get; }
        public Version Version { get; set; }

        protected override string DebuggerDisplay => "Version: " + Version;

        public ABCFile()
        {
            _classByQNameCache = new Dictionary<ASMultiname, List<ASClass>>();
            _instanceByConstructorCache = new Dictionary<string, ASInstance>();

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
            _classByQNameCache.EnsureCapacity(Instances.Count);
            _instanceByConstructorCache.EnsureCapacity(Instances.Count);

            PopulateList(Classes, ReadClass, Instances.Count);
            PopulateList(Scripts, ReadScript);
            PopulateList(MethodBodies, ReadMethodBody);

            _classByQNameCache.TrimExcess();
            _instanceByConstructorCache.TrimExcess();
        }

        public void ResetCache()
        {
            _classByQNameCache.Clear();
            _instanceByConstructorCache.Clear();
            foreach (ASClass @class in Classes)
            {
                CacheByNaming(@class);
            }
            _classByQNameCache.TrimExcess();
            _instanceByConstructorCache.TrimExcess();
        }
        private void CacheByNaming(ASClass @class)
        {
            if (!string.IsNullOrWhiteSpace(@class.Instance.Constructor.Name))
            {
                string prefix = null;
                if (!string.IsNullOrWhiteSpace(@class.QName.Namespace.Name) && !@class.QName.Namespace.Name.StartsWith("_-"))
                {
                    prefix = @class.QName.Namespace.Name + ".";
                }
                _instanceByConstructorCache.Add(prefix + @class.Instance.Constructor.Name, @class.Instance);
            }

            if (!_classByQNameCache.TryGetValue(@class.QName, out List<ASClass> classes))
            {
                classes = new List<ASClass>();
                _classByQNameCache.Add(@class.QName, classes);
            }
            classes.Add(@class);
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
            int index = recycle ? valueList.IndexOf(value) : -1;
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
            if (multiname == null)
            {
                return Enumerable.Empty<ASClass>();
            }
            return _classByQNameCache.GetValueOrDefault(multiname) ?? Enumerable.Empty<ASClass>();
        }
        public IEnumerable<ASClass> GetClasses(string qualifiedName) => GetClasses(GetMultiname(qualifiedName));

        public IEnumerable<ASInstance> GetInstances(ASMultiname multiname) => GetClasses(multiname).Select(c => c.Instance);
        public IEnumerable<ASInstance> GetInstances(string qualifiedName) => GetInstances(GetMultiname(qualifiedName));

        public ASInstance GetInstanceByConstructor(string constructorName) => _instanceByConstructorCache.GetValueOrDefault(constructorName);

        private ASMethod ReadMethod(int index) => new(this, _input);
        private ASMetadata ReadMetadata(int index) => new(this, _input);
        private ASInstance ReadInstance(int index) => new(this, _input);
        private ASClass ReadClass(int index)
        {
            var @class = new ASClass(this, _input)
            {
                InstanceIndex = index
            };
            CacheByNaming(@class);
            return @class;
        }
        private ASScript ReadScript(int index) => new(this, _input);
        private ASMethodBody ReadMethodBody(int index) => new(this, _input);

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
            list.Capacity = count < 0 ? _input.ReadInt30() : count;
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
                _classByQNameCache.Clear();
                _instanceByConstructorCache.Clear();

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