# Flazzy
.NET 5.0+ library for editing, and viewing the contents inside a SWF file. There are only a couple of supported tags that this library can manipulate, but instead of listing them here, it would be better just to link you [here](/Flazzy/Tags). It would be easier to see the directory of where the supported tags are contained in this project, rather than having to update this file when a new tag is added.

## Now I know my ABC's
There aren't a lot of tags supported yet in this library for modifiyng, but there is one important one that is probably the main reason for the existence of this library. This is the [DoABCTag](/Flazzy/Tags/DoABCTag.cs), or more accurately the [ABCFile](/Flazzy/ABC/ABCFile.cs) type.  

The ABCFile type provides us will all the methods, classes, scripts, and all other good stuff that allow the SWF file to run like it does.
A property named **Pool** is contained in this type, this property is of type [ASConstantPool](/Flazzy/ABC/ASConstantPool.cs), which contains collections of constant values used throughout the ABC file.  

Almost everything is index-based in the ABC file, if a class has a name **foo**, it will instead point to the index of **foo** in the Strings collection in the constant pool. The same format applies to other constant types found throughout the ABC file, although there are some cases where the raw value is simply provided to the type for reading. Therefore, be careful when inserting, or removing values from any collection in the constant pool, as this will undoubtedly affect every object utilizing that collection.  

There are many types you can manipulate in the [ABC](/Flazzy/ABC) namespace, but we can do better if we go deeper. If you want to continue learning about other types found in the ABCFile, it would be in your best interest to read some of stuff in the **AVM2 Format** document starting on page **4.2**.

## ActionScript Virtual Machine
This library provides functionality for adding, removing, or modifying instructions inside of the method bodies. Provided in the [AVM2](/Flazzy/ABC/AVM2) namespace, is a type called [ASCode](/Flazzy/ABC/AVM2/ASCode.cs). This type inherits from IList\<ASInstruction\>, allowing for safe modification of any instruction within the container. By safe modification I mean it takes care of automatically adjusting new exit positions for every jump/switch instruction. This allows us to freely remove/add instructions without having to worry about setting a new offset value for the jump instruction.  

The **ASCode** type also allows for basic control-flow deobfuscation, you can acheive this by calling the **Deobfuscate()** method after the type is initialized. For a list of supported instructions see [here](/Flazzy/ABC/AVM2/Instructions).

## Usage/Tutorials
For some tutorials to get you started, proceed to the project wiki.
