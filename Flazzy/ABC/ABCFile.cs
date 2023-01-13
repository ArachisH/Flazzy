using Flazzy.IO;

namespace Flazzy.ABC;

public class ABCFile : IFlashItem, IDisposable
{
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
    public ABCFile(ref FlashReader input)
        : this()
    {
        Version = new Version(
            minor: input.ReadUInt16(), 
            major: input.ReadUInt16());
        
        Pool = new ASConstantPool(this, ref input);

        Methods.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < Methods.Capacity; i++)
        {
            Methods.Add(new ASMethod(this, ref input));
        }

        Metadata.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < Metadata.Capacity; i++)
        {
            Metadata.Add(new ASMetadata(this, ref input));
        }

        Instances.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < Instances.Capacity; i++)
        {
            Instances.Add(new ASInstance(this, ref input));
        }

        _classByQNameCache.EnsureCapacity(Instances.Count);
        _instanceByConstructorCache.EnsureCapacity(Instances.Count);

        Classes.Capacity = Instances.Count;
        for (int i = 0; i < Classes.Capacity; i++)
        {
            var @class = new ASClass(this, ref input)
            {
                InstanceIndex = i
            };
            CacheByNaming(@class);
            Classes.Add(@class);
        }

        Scripts.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < Scripts.Capacity; i++)
        {
            Scripts.Add(new ASScript(this, ref input));
        }

        MethodBodies.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < MethodBodies.Capacity; i++)
        {
            MethodBodies.Add(new ASMethodBody(this, ref input));
        }

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
            if (!string.IsNullOrWhiteSpace(@class.QName.Namespace.Name) &&
                !@class.QName.Namespace.Name.StartsWith("_-", StringComparison.OrdinalIgnoreCase))
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
            index = valueList.Count;
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

    private ASMultiname GetMultiname(string qualifiedName)
    {
        foreach (ASMultiname multiname in Pool.GetMultinames(qualifiedName))
        {
            if (multiname.Kind == MultinameKind.QName) return multiname;
        }
        return null;
    }

    public int GetSize()
    {
        int size = 0;
        size += sizeof(ushort);
        size += sizeof(ushort);
        size += Pool.GetSize();

        size += FlashWriter.GetEncodedIntSize(Methods.Count);
        for (int i = 0; i < Methods.Count; i++)
        {
            size += Methods[i].GetSize();
        }

        size += FlashWriter.GetEncodedIntSize(Metadata.Count);
        for (int i = 0; i < Metadata.Count; i++)
        {
            size += Metadata[i].GetSize();
        }

        size += FlashWriter.GetEncodedIntSize(Instances.Count);
        for (int i = 0; i < Instances.Count; i++)
        {
            size += Instances[i].GetSize();
        }
        for (int i = 0; i < Classes.Count; i++)
        {
            size += Classes[i].GetSize();
        }

        size += FlashWriter.GetEncodedIntSize(Scripts.Count);
        for (int i = 0; i < Scripts.Count; i++)
        {
            size += Scripts[i].GetSize();
        }

        size += FlashWriter.GetEncodedIntSize(MethodBodies.Count);
        for (int i = 0; i < MethodBodies.Count; i++)
        {
            size += MethodBodies[i].GetSize();
        }
        return size;
    }
    public void WriteTo(ref FlashWriter output)
    {
        output.Write((ushort)Version.Minor);
        output.Write((ushort)Version.Major);

        Pool.WriteTo(ref output);

        WriteTo(ref output, Methods);
        WriteTo(ref output, Metadata);
        WriteTo(ref output, Instances);
        WriteTo(ref output, Classes, false);
        WriteTo(ref output, Scripts);
        WriteTo(ref output, MethodBodies);
    }

    public void Dispose()
    {
        Dispose(true);
    }
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
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

    public override string ToString() => "Version: " + Version;

    private static void WriteTo<T>(ref FlashWriter output, List<T> list, bool writeCount = true)
        where T : IFlashItem
    {
        if (writeCount)
        {
            output.WriteEncodedInt(list.Count);
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i].WriteTo(ref output);
        }
    }
}
