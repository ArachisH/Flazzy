# Flazzy
[![CI Workflow](https://github.com/ArachisH/Flazzy/actions/workflows/ci.yaml/badge.svg)](https://github.com/ArachisH/Flazzy/actions/workflows/ci.yaml)
[![NuGet](https://img.shields.io/nuget/v/Hypo.Flazzy?label=NuGet)](https://www.nuget.org/packages/Hypo.Flazzy)
[![NuGet](https://img.shields.io/nuget/vpre/Hypo.Flazzy?label=NuGet%20(Pre))](https://www.nuget.org/packages/Hypo.Flazzy)
![License](https://img.shields.io/github/license/ArachisH/Flazzy?label=License)

A .NET library for reading, writing, and analyzing SWF files.

Flazzy is primarily designed for Flash reverse-engineering and deobfuscation workflows. It can disassemble SWFs into tag objects, parse embedded ABC data, inspect and rewrite AVM2 bytecode, normalize invalid AS3 identifiers, and rebuild patched files.

Although the project includes convenient high-level entry points such as `ShockwaveFlash` and `ABCFile`, it also exposes lower-level structures for tags, records, constant pools, multinames, methods, and instructions when you need direct control over the file format.

## What It Does
- Reads and disassembles SWF files for inspection and patching
- Parses common SWF tags while preserving unsupported ones as raw data
- Extracts and parses `DoABC` blocks
- Models ABC structures such as classes, scripts, methods, metadata, namespaces, and multinames
- Parses AVM2 bytecode into editable instruction objects
- Deobfuscates AVM2 control flow
- Renames invalid or obfuscated AS3 type and namespace names into compiler-safe identifiers
- Rebuilds modified SWF and ABC data back into binary form
- Decrypts Flash content protected by newer AIR/HARMAN runtime encryption flows

## Quick Start
Install the package:

```powershell
dotnet add package Hypo.Flazzy
```

Load a SWF and inspect its contents:

```cs
using Flazzy;
using Flazzy.Tags;

using var swf = new ShockwaveFlash("client.swf");
swf.Disassemble();

Console.WriteLine($"{swf.Signature} v{swf.Version}");
Console.WriteLine($"Tags: {swf.Tags.Count}");

foreach (TagItem tag in swf.Tags)
{
    Console.WriteLine(tag.Kind);
}
```

## How It Works
At the SWF level, `ShockwaveFlash` reads the file header, handles supported compression, parses the frame record, and then walks each tag into a concrete `TagItem` implementation when one is available.

ABC data embedded in `DoABC` tags can then be loaded into `ABCFile`, which exposes higher-level structures such as classes, scripts, methods, constant pools, and method bodies. For AVM2-level analysis, `ASMethodBody.ParseCode()` produces an `ASCode` instruction list that can be inspected, modified, and rewritten.

When you write the file back out, Flazzy rebuilds the SWF container and emits modified tag and ABC structures in binary form.

## Working with ABC
ABC blocks can be loaded directly from `DoABC` tags:

```cs
using Flazzy;
using Flazzy.ABC;
using Flazzy.Tags;

using var swf = new ShockwaveFlash("client.swf");
swf.Disassemble();

foreach (DoABCTag tag in swf.Tags.OfType<DoABCTag>())
{
    using var abc = new ABCFile(tag.ABCData);

    Console.WriteLine($"ABC version: {abc.Version}");
    Console.WriteLine($"Classes: {abc.Classes.Count}");
    Console.WriteLine($"Methods: {abc.Methods.Count}");

    foreach (var method in abc.Methods.Where(m => !string.IsNullOrWhiteSpace(m.Name)).Take(10))
    {
        Console.WriteLine(method.ToAS3());
    }
}
```

Useful entry points:
- `ABCFile.Classes`
- `ABCFile.Methods`
- `ABCFile.MethodBodies`
- `ABCFile.Scripts`
- `ABCFile.Pool`

## Working with AVM2 Bytecode
Method bodies can be parsed into instruction lists:

```cs
using Flazzy;
using Flazzy.ABC;
using Flazzy.Tags;

using var swf = new ShockwaveFlash("client.swf");
swf.Disassemble();

DoABCTag abcTag = swf.Tags.OfType<DoABCTag>().First();
using var abc = new ABCFile(abcTag.ABCData);

foreach (var body in abc.MethodBodies)
{
    ASCode code = body.ParseCode();
    code.Deobfuscate();

    Console.WriteLine(body.Method);
    Console.WriteLine($"Instruction count: {code.Count}");
}
```

This is useful when you need to:
- Inspect AVM2 opcodes
- Remove simple obfuscation patterns
- Rewrite branches or constants
- Rebuild modified method bodies back into the containing ABC block

## Rebuilding Files
Once tags or ABC data have been modified, the SWF can be written back out:

```cs
using Flazzy;

using var swf = new ShockwaveFlash("client.swf");
swf.Disassemble();

/* modify tags, ABC data, or other structures here */

using var output = File.Create("client.modified.swf");
swf.CopyTo(output, swf.Compression);
```

## Decrypting Encrypted SWFs
Some newer AIR/HARMAN Flash runtimes use encrypted SWF content at runtime. Flazzy exposes `FlashCrypto` so encrypted data can be decrypted before normal SWF parsing begins.

```cs
using Flazzy.Tools;

Span<byte> buffer = /* encrypted SWF data */;

int decryptedLength = FlashCrypto.Decrypt(ref buffer, out int writtenOffset);
Span<byte> decryptedData = buffer.Slice(writtenOffset, decryptedLength);
```

`FlashCrypto.Decrypt` modifies the provided buffer in-place. After decryption, `writtenOffset` becomes the new starting position of the SWF data, and the returned length describes the full decrypted payload.

## Important Notes
- Unknown tags are preserved through `UnknownTag`, which makes partial-format support practical for patching workflows.
- `ShockwaveFlash.Disassemble()` consumes the underlying input stream and populates the in-memory tag list.
- `DoABCTag` exposes raw `ABCData`, which you can parse into `ABCFile` when needed.
- `FlashCrypto` is intended for public use when working with encrypted Flash content produced for newer AIR/HARMAN runtimes.
- The project is useful both as a high-level SWF library and as a lower-level format toolkit.

## Supported Formats and Limitations
Currently supported:
- Uncompressed SWF
- ZLib-compressed SWF
- Round-tripping unsupported tags as raw data
- ABC parsing and AVM2 instruction modeling

Not currently supported:
- LZMA-compressed SWF
- Complete coverage of every SWF tag type

## Core API Surface
The main entry points are:
- `ShockwaveFlash`
- `TagItem` and concrete tag types under `Flazzy.Tags`
- `ABCFile`
- `ASMethodBody`
- `ASCode`
- `FlashReader` and `FlashWriter`

## Documentation
This file should work as the project overview. More detailed reference material belongs in the wiki and specification sheets:

- [Wiki](https://github.com/ArachisH/Flazzy/wiki)
- [Specifications](https://github.com/ArachisH/Flazzy/tree/develop/Specifications)

## Contributing
Contributions are welcome.

- Create a `feat/` or `feature/` branch from `develop`
- Open pull requests back into `develop`
- Open an issue for bugs, regressions, or feature requests
- Include clear reproduction details when reporting parsing, rewriting, or deobfuscation issues
