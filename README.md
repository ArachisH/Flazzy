# Flazzy
[![Build CI](https://github.com/ArachisH/Flazzy/actions/workflows/build.yaml/badge.svg)](https://github.com/ArachisH/Flazzy/actions)
[![NuGet](https://img.shields.io/nuget/v/Hypo.Flazzy?label=NuGet)](https://www.nuget.org/packages/Hypo.Flazzy)
![License](https://img.shields.io/github/license/ArachisH/Flazzy?label=License)

.NET library for [dis]assembling shockwave flash files(swf).

# Features
* [Tag Parsing](https://github.com/ArachisH/Flazzy/tree/develop/Flazzy/Tags)
* AS3 Decompiler
* AS3 Control Flow Deobfuscation
* AS3 Class & Namespace String Deobfuscation
 
# Usage & Building
The documentation for how to utilize the full functionality of the library can be found in the repository [Wiki](https://github.com/ArachisH/Flazzy/wiki). The specification sheets that were used for this project can also be found [here](https://github.com/ArachisH/Flazzy/tree/develop/Specifications).
If you're looking to build from source you can clone it, and run a simple dotnet build command.
```bash
git clone https://github.com/ArachisH/Flazzy.git && cd Flazzy
dotnet build
```

# Contributing & Issues
Any contribution is welcomed, so long as you base your PR off of the develop branch. If you have any problems, or even feature requests, make an issue to make it easier for tracking/automation.