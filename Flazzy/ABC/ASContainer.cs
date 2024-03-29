﻿using Flazzy.IO;

namespace Flazzy.ABC;

public abstract class ASContainer : IFlashItem
{
    public ABCFile ABC { get; }

    public List<ASTrait> Traits { get; }

    public virtual bool IsStatic { get; }
    public abstract ASMultiname QName { get; }

    public ASContainer(ABCFile abc)
    {
        ABC = abc;
        Traits = new List<ASTrait>();
    }

    public ASTrait AddMethod(ASMethod method, string qualifiedName)
    {
        int methodIndex = ABC.AddMethod(method);
        int qNameIndex = AddPublicQualifiedName(qualifiedName);

        var trait = new ASTrait(ABC)
        {
            Kind = TraitKind.Method,
            QNameIndex = qNameIndex,
            MethodIndex = methodIndex
        };
        method.Trait = trait;
        method.Container = this;

        Traits.Add(trait);
        return trait;
    }
    public ASTrait AddSlot(string qualifiedName, string typeQualifiedName)
    {
        var trait = new ASTrait(ABC)
        {
            Kind = TraitKind.Slot,
            QNameIndex = AddPublicQualifiedName(qualifiedName),
            TypeIndex = ABC.Pool.GetMultinameIndex(typeQualifiedName)
        };

        Traits.Add(trait);
        return trait;
    }

    public IEnumerable<ASMethod> GetMethods()
    {
        return GetTraits(TraitKind.Method, TraitKind.Getter, TraitKind.Setter).Select(t => t.Method);
    }
    public IEnumerable<ASMethod> GetMethods(string qualifiedName) => GetMethods(qualifiedName, null, null);
    public IEnumerable<ASMethod> GetMethods(string qualifiedName, string returnTypeName) => GetMethods(qualifiedName, returnTypeName, null);
    public IEnumerable<ASMethod> GetMethods(string qualifiedName, string returnTypeName, int paramCount) => GetMethods(qualifiedName, returnTypeName, new string[paramCount]);
    public IEnumerable<ASMethod> GetMethods(string qualifiedName, string returnTypeName, string[] paramTypeNames)
    {
        foreach (ASMethod method in GetMethods())
        {
            if (qualifiedName != null && method.Trait.QName.Name != qualifiedName) continue;
            if (returnTypeName != null && method.ReturnType?.Name != returnTypeName) continue;
            if (paramTypeNames != null)
            {
                if (method.Parameters.Count != paramTypeNames.Length) continue;

                bool isContinuing = false;
                for (int i = 0; i < paramTypeNames.Length; i++)
                {
                    if (paramTypeNames[i] == null) continue;
                    if (method.Parameters[i].Type.Name != paramTypeNames[i])
                    {
                        isContinuing = true;
                        break;
                    }
                }
                if (isContinuing) continue;
            }
            yield return method;
        }
    }

    public ASMethod GetMethod(string qualifiedName) => GetMethods(qualifiedName).FirstOrDefault();
    public ASMethod GetMethod(string qualifiedName, string returnTypeName) => GetMethods(qualifiedName, returnTypeName).FirstOrDefault();
    public ASMethod GetMethod(string qualifiedName, string returnTypeName, int paramCount) => GetMethods(qualifiedName, returnTypeName, paramCount).FirstOrDefault();
    public ASMethod GetMethod(string qualifiedName, string returnTypeName, string[] paramTypeNames) => GetMethods(qualifiedName, returnTypeName, paramTypeNames).FirstOrDefault();

    public IEnumerable<ASTrait> GetGetters()
    {
        return GetTraits(TraitKind.Getter);
    }
    public IEnumerable<ASTrait> GetGetters(string returnTypeName)
    {
        return GetGetters()
            .Where(g => g.Type.Name == returnTypeName);
    }

    public IEnumerable<ASTrait> GetSlotTraits(string returnTypeName)
    {
        return GetTraits(TraitKind.Slot)
            .Where(sct => (sct.Type?.Name ?? "*") == returnTypeName);
    }
    public IEnumerable<ASTrait> GetConstantTraits(string returnTypeName)
    {
        return GetTraits(TraitKind.Constant)
            .Where(sct => (sct.Type?.Name ?? "*") == returnTypeName);
    }

    public ASTrait GetSlot(string qualifiedName)
    {
        return GetTraits(TraitKind.Slot).FirstOrDefault(st => st.QName.Name == qualifiedName);
    }
    public ASTrait GetGetter(string qualifiedName)
    {
        return GetGetters().FirstOrDefault(g => g.QName.Name == qualifiedName);
    }
    public ASTrait GetConstant(string qualifiedName)
    {
        return GetTraits(TraitKind.Constant).FirstOrDefault(ct => ct.QName.Name == qualifiedName);
    }

    public IEnumerable<ASTrait> GetTraits(params TraitKind[] kinds)
    {
        return (kinds?.Length ?? 0) == 0 ?
            Enumerable.Empty<ASTrait>() :
            Traits.Where(t => kinds.Contains(t.Kind));
    }

    private int AddPublicQualifiedName(string qualifiedName)
    {
        var qName = new ASMultiname(ABC.Pool)
        {
            NameIndex = ABC.Pool.AddConstant(qualifiedName),
            Kind = MultinameKind.QName,
            NamespaceIndex = 1 // Public
        };
        return ABC.Pool.AddConstant(qName);
    }

    protected void PopulateTraits(ref SpanFlashReader input)
    {
        Traits.Capacity = input.ReadEncodedInt();
        for (int i = 0; i < Traits.Capacity; i++)
        {
            var trait = new ASTrait(ABC, ref input)
            {
                IsStatic = IsStatic
            };

            if (trait.Kind is TraitKind.Method or TraitKind.Getter or TraitKind.Setter)
            {
                trait.Method.Container = this;
            }

            Traits.Add(trait);
        }
    }
    public virtual int GetSize()
    {
        int size = 0;
        size += SpanFlashWriter.GetEncodedIntSize(Traits.Count);
        for (int i = 0; i < Traits.Count; i++)
        {
            size += Traits[i].GetSize();
        }
        return size;
    }
    public virtual void WriteTo(ref SpanFlashWriter output)
    {
        output.WriteEncodedInt(Traits.Count);
        for (int i = 0; i < Traits.Count; i++)
        {
            Traits[i].WriteTo(ref output);
        }
    }

    public override string ToString()
    {
        int methodCount = Traits.Count(
            t => t.Kind is TraitKind.Method or TraitKind.Getter or TraitKind.Setter);

        int slotCount = Traits.Count(t => t.Kind is TraitKind.Slot);
        int constantCount = Traits.Count(t => t.Kind is TraitKind.Constant);

        return $"{QName}, Traits: {Traits.Count}";
    }
}