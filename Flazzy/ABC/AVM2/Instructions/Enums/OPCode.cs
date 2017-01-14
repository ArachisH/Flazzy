namespace Flazzy.ABC.AVM2.Instructions
{
    public enum OPCode
    {
        /// <summary>
        /// Add two values.
        /// </summary>
        Add = 0xa0,
        /// <summary>
        /// Add two integer values.
        /// </summary>
        Add_i = 0xc5,
        /// <summary>
        /// Apply type/generic parameters.
        /// </summary>
        ApplyType = 0x53,
        /// <summary>
        /// Return the same value, or null if not of the specified type.
        /// </summary>
        AsType = 0x86,
        /// <summary>
        /// Return the same value, or null if not of the specified type.
        /// </summary>
        AsTypeLate = 0x87,
        /// <summary>
        /// Bitwise AND(&amp;).
        /// </summary>
        BitAnd = 0xa8,
        /// <summary>
        /// Bitwise NOT(~).
        /// </summary>
        BitNot = 0x97,
        /// <summary>
        /// Bitwise OR(|).
        /// </summary>
        BitOr = 0xa9,
        /// <summary>
        /// Bitwise exclusive OR(^).
        /// </summary>
        BitXor = 0xaa,
        /// <summary>
        /// Call a closure.
        /// </summary>
        Call = 0x41,
        /// <summary>
        /// Call a method identified by index in the object’s method table.
        /// </summary>
        CallMethod = 0x43,
        /// <summary>
        /// Call a property.
        /// </summary>
        CallProperty = 0x46,
        /// <summary>
        /// Call a property.
        /// </summary>
        CallPropLex = 0x4c,
        /// <summary>
        /// Call a property, discarding the return value.
        /// </summary>
        CallPropVoid = 0x4f,
        /// <summary>
        /// Call a method identified by index in the <see cref="ABCFile.Methods"/> property.
        /// </summary>
        CallStatic = 0x44,
        /// <summary>
        /// Call a method on a base class.
        /// </summary>
        CallSuper = 0x45,
        /// <summary>
        /// Call a method on a base class, discarding the return value.
        /// </summary>
        CallSuperVoid = 0x4e,
        /// <summary>
        /// Check to make sure an object can have a filter operation performed on it.
        /// </summary>
        CheckFilter = 0x78,
        /// <summary>
        /// Coerce a value to a specified type.
        /// </summary>
        Coerce = 0x80,
        /// <summary>
        /// Coerce a value to the any type(*).
        /// </summary>
        Coerce_a = 0x82,
        /// <summary>
        /// Coerce a value to a string.
        /// </summary>
        Coerce_s = 0x85,
        /// <summary>
        /// Construct an instance.
        /// </summary>
        Construct = 0x42,
        /// <summary>
        /// Construct a property.
        /// </summary>
        ConstructProp = 0x4a,
        /// <summary>
        /// Construct an instance of the base class.
        /// </summary>
        ConstructSuper = 0x49,
        /// <summary>
        /// Convert a value to a bool.
        /// </summary>
        Convert_b = 0x76,
        /// <summary>
        /// Convert a value to an integer.
        /// </summary>
        Convert_i = 0x73,
        /// <summary>
        /// Convert a value to a double.
        /// </summary>
        Convert_d = 0x75,
        /// <summary>
        /// Convert a value to an object.
        /// </summary>
        Convert_o = 0x77,
        /// <summary>
        /// Convert a value to an unsigned integer.
        /// </summary>
        Convert_u = 0x74,
        /// <summary>
        /// Convert a value to a string.
        /// </summary>
        Convert_s = 0x70,
        /// <summary>
        /// Debugging info.
        /// </summary>
        Debug = 0xef,
        /// <summary>
        /// Debugging line number info.
        /// </summary>
        DebugFile = 0xf1,
        /// <summary>
        /// Debugging line number info.
        /// </summary>
        DebugLine = 0xf0,
        /// <summary>
        /// Decrement a local register value.
        /// </summary>
        DecLocal = 0x94,
        /// <summary>
        /// Decrement a local register value.
        /// </summary>
        DecLocal_i = 0xc3,
        /// <summary>
        /// Decrement a value.
        /// </summary>
        Decrement = 0x93,
        /// <summary>
        /// Decrement an integer value.
        /// </summary>
        Decrement_i = 0xc1,
        /// <summary>
        /// Delete a property.
        /// </summary>
        DeleteProperty = 0x6a,
        /// <summary>
        /// Divide two values.
        /// </summary>
        Divide = 0xa3,
        /// <summary>
        /// Duplicates the top value on the stack.
        /// </summary>
        Dup = 0x2a,
        /// <summary>
        /// Sets the default XML namespace.
        /// </summary>
        Dxns = 0x06,
        /// <summary>
        /// Sets the default XML namespace with a value determined at runtime.
        /// </summary>
        DxnsLate = 0x07,
        /// <summary>
        /// Compare two values.
        /// </summary>
        Equals = 0xab,
        /// <summary>
        /// Escape an xml attribute.
        /// </summary>
        Esc_XAttr = 0x72,
        /// <summary>
        /// Escape an xml element.
        /// </summary>
        Esc_XElem = 0x71,
        /// <summary>
        /// Search the scope stack for a property.
        /// </summary>
        FindProperty = 0x5e,
        /// <summary>
        /// Find a property.
        /// </summary>
        FindPropStrict = 0x5d,
        /// <summary>
        /// Get descendants.
        /// </summary>
        GetDescendants = 0x59,
        /// <summary>
        /// Gets the global scope.
        /// </summary>
        GetGlobalScope = 0x64,
        /// <summary>
        /// Get the value of a slot on the global scope.
        /// </summary>
        GetGlobalSlot = 0x6e,
        /// <summary>
        /// Find and get a property.
        /// </summary>
        GetLex = 0x60,
        /// <summary>
        /// Get a local register.
        /// </summary>
        GetLocal = 0x62,
        /// <summary>
        /// Get local register 0.
        /// </summary>
        GetLocal_0 = 0xd0,
        /// <summary>
        /// Get local register 1.
        /// </summary>
        GetLocal_1 = 0xd1,
        /// <summary>
        /// Get local register 2.
        /// </summary>
        GetLocal_2 = 0xd2,
        /// <summary>
        /// Get local register 3.
        /// </summary>
        GetLocal_3 = 0xd3,
        /// <summary>
        /// Get a property.
        /// </summary>
        GetProperty = 0x66,
        /// <summary>
        /// Get a scope object.
        /// </summary>
        GetScopeObject = 0x65,
        /// <summary>
        /// Get the value of a slot.
        /// </summary>
        GetSlot = 0x6c,
        /// <summary>
        /// Gets a property from a base class.
        /// </summary>
        GetSuper = 0x04,
        /// <summary>
        /// Determine if one value is greater than or equal to another.
        /// </summary>
        GreaterEquals = 0xb0,
        /// <summary>
        /// Determine if one value is greater than another.
        /// </summary>
        GreaterThan = 0xaf,
        /// <summary>
        /// Determine if the given object has any more properties.
        /// </summary>
        HasNext = 0x1f,
        /// <summary>
        /// Determine if the given object has any more properties.
        /// </summary>
        HasNext2 = 0x32,
        /// <summary>
        /// Branch if the first value is equal to the second value.
        /// </summary>
        IfEq = 0x13,
        /// <summary>
        /// Branch if false.
        /// </summary>
        IfFalse = 0x12,
        /// <summary>
        /// Branch if the first value is greater than or equal to the second value.
        /// </summary>
        IfGe = 0x18,
        /// <summary>
        /// Branch if the first value is greater than the second value.
        /// </summary>
        IfGt = 0x17,
        /// <summary>
        /// Branch if the first value is less than or equal to the second value.
        /// </summary>
        IfLe = 0x16,
        /// <summary>
        /// Branch if the first value is less than the second value.
        /// </summary>
        IfLt = 0x15,
        /// <summary>
        /// Branch if the first value is not greater than or equal to the second value.
        /// </summary>
        IfNGe = 0x0f,
        /// <summary>
        /// Branch if the first value is not greater than the second value.
        /// </summary>
        IfNGt = 0x0e,
        /// <summary>
        /// Branch if the first value is not less than or equal to the second value.
        /// </summary>
        IfNLe = 0x0d,
        /// <summary>
        /// Branch if the first value is not less than the second value.
        /// </summary>
        IfNLt = 0x0c,
        /// <summary>
        /// Branch if the first value is not equal to the second value.
        /// </summary>
        IfNe = 0x14,
        /// <summary>
        /// Branch if the first value is equal to the second value.
        /// </summary>
        IfStrictEq = 0x19,
        /// <summary>
        /// Branch if the first value is not equal to the second value.
        /// </summary>
        IfStrictNE = 0x1a,
        /// <summary>
        /// Branch if true.
        /// </summary>
        IfTrue = 0x11,
        /// <summary>
        /// Determine whether an object has a named property.
        /// </summary>
        In = 0xb4,
        /// <summary>
        /// Increment a local register value.
        /// </summary>
        IncLocal = 0x92,
        /// <summary>
        /// Increment a local register value.
        /// </summary>
        IncLocal_i = 0xc2,
        /// <summary>
        /// Increment a value.
        /// </summary>
        Increment = 0x91,
        /// <summary>
        /// Increment an integer value.
        /// </summary>
        Increment_i = 0xc0,
        /// <summary>
        /// Initialize a property.
        /// </summary>
        InitProperty = 0x68,
        /// <summary>
        /// Check the prototype chain of an object for the existence of a type.
        /// </summary>
        InstanceOf = 0xb1,
        /// <summary>
        /// Checks whether an object is of a certain type.
        /// </summary>
        IsType = 0xb2,
        /// <summary>
        /// Checks whether an object is of a certain type.
        /// </summary>
        IsTypeLate = 0xb3,
        /// <summary>
        /// Unconditional branch.
        /// </summary>
        Jump = 0x10,
        /// <summary>
        /// Kills a local register.
        /// </summary>
        Kill = 0x08,
        /// <summary>
        /// Do nothing.
        /// </summary>
        Label = 0x09,
        /// <summary>
        /// Determine if one value is less than or equal to another.
        /// </summary>
        LessEquals = 0xae,
        /// <summary>
        /// Determine of one value is less than another.
        /// </summary>
        LessThan = 0xad,
        /// <summary>
        /// Jump to different locations based on an index.
        /// </summary>
        LookUpSwitch = 0x1b,
        /// <summary>
        /// Bitwise left shift.
        /// </summary>
        LShift = 0xa5,
        /// <summary>
        /// Perform modulo division on two values.
        /// </summary>
        Modulo = 0xa4,
        /// <summary>
        /// Multiply two values.
        /// </summary>
        Multiply = 0xa2,
        /// <summary>
        /// Multiply two integer values.
        /// </summary>
        Multiply_i = 0xc7,
        /// <summary>
        /// Negate a value.
        /// </summary>
        Negate = 0x90,
        /// <summary>
        /// Negate an integer value.
        /// </summary>
        Negate_i = 0xc4,
        /// <summary>
        /// Create a new activation object.
        /// </summary>
        NewActivation = 0x57,
        /// <summary>
        /// Create a new array.
        /// </summary>
        NewArray = 0x56,
        /// <summary>
        /// Create a new catch scope.
        /// </summary>
        NewCatch = 0x5a,
        /// <summary>
        /// Create a new class.
        /// </summary>
        NewClass = 0x58,
        /// <summary>
        /// Create a new function object.
        /// </summary>
        NewFunction = 0x40,
        /// <summary>
        /// Create a new object.
        /// </summary>
        NewObject = 0x55,
        /// <summary>
        /// Get the name of the next property when iterating over an object.
        /// </summary>
        NextName = 0x1e,
        /// <summary>
        /// Get the name of the next property when iterating over an object.
        /// </summary>
        NextValue = 0x23,
        /// <summary>
        /// Do nothing.
        /// </summary>
        Nop = 0x02,
        /// <summary>
        /// Boolean negation.
        /// </summary>
        Not = 0x96,
        /// <summary>
        /// Pop the top value from the stack.
        /// </summary>
        Pop = 0x29,
        /// <summary>
        /// Pop a scope off of the scope stack.
        /// </summary>
        PopScope = 0x1d,
        /// <summary>
        /// Push a byte value.
        /// </summary>
        PushByte = 0x24,
        /// <summary>
        /// Push a double value onto the stack.
        /// </summary>
        PushDouble = 0x2f,
        /// <summary>
        /// Push false.
        /// </summary>
        PushFalse = 0x27,
        /// <summary>
        /// Push an int value onto the stack.
        /// </summary>
        PushInt = 0x2d,
        /// <summary>
        /// Push a namespace.
        /// </summary>
        PushNamespace = 0x31,
        /// <summary>
        /// Push NaN(Not A Number).
        /// </summary>
        PushNan = 0x28,
        /// <summary>
        /// Push null.
        /// </summary>
        PushNull = 0x20,
        /// <summary>
        /// Push an object onto the scope stack.
        /// </summary>
        PushScope = 0x30,
        /// <summary>
        /// Push a short value.
        /// </summary>
        PushShort = 0x25,
        /// <summary>
        /// Push a string value onto the stack.
        /// </summary>
        PushString = 0x2c,
        /// <summary>
        /// Push true.
        /// </summary>
        PushTrue = 0x26,
        /// <summary>
        /// Push an unsigned int value onto the stack.
        /// </summary>
        PushUInt = 0x2e,
        /// <summary>
        /// Push undefined.
        /// </summary>
        PushUndefined = 0x21,
        /// <summary>
        /// Push a with scope onto the scope stack.
        /// </summary>
        PushWith = 0x1c,
        /// <summary>
        /// Return a value from a method.
        /// </summary>
        ReturnValue = 0x48,
        /// <summary>
        /// Return from a method.
        /// </summary>
        ReturnVoid = 0x47,
        /// <summary>
        /// Signed bitwise right shift.
        /// </summary>
        RShift = 0xa6,
        /// <summary>
        /// Set a local register.
        /// </summary>
        SetLocal = 0x63,
        /// <summary>
        /// Set local register 0.
        /// </summary>
        SetLocal_0 = 0xd4,
        /// <summary>
        /// Set local register 1.
        /// </summary>
        SetLocal_1 = 0xd5,
        /// <summary>
        /// Set local register 2.
        /// </summary>
        SetLocal_2 = 0xd6,
        /// <summary>
        /// Set local register 3.
        /// </summary>
        SetLocal_3 = 0xd7,
        /// <summary>
        /// Set the value of a slot on the global scope.
        /// </summary>
        SetGlobalSlot = 0x6f,
        /// <summary>
        /// Set a property.
        /// </summary>
        SetProperty = 0x61,
        /// <summary>
        /// Set the value of a slot.
        /// </summary>
        SetSlot = 0x6d,
        /// <summary>
        /// Sets a property in a base class.
        /// </summary>
        SetSuper = 0x05,
        /// <summary>
        /// Compare two values strictly.
        /// </summary>
        StrictEquals = 0xac,
        /// <summary>
        /// Subtract one value from another.
        /// </summary>
        Subtract = 0xa1,
        /// <summary>
        /// Subtract an integer value from another integer value.
        /// </summary>
        Subtract_i = 0xc6,
        /// <summary>
        /// Swap the top two operands on the stack.
        /// </summary>
        Swap = 0x2b,
        /// <summary>
        /// Throws an exception.
        /// </summary>
        Throw = 0x03,
        /// <summary>
        /// Get the type name of a value.
        /// </summary>
        TypeOf = 0x95,
        /// <summary>
        /// Unsigned bitwise right shift.
        /// </summary>
        URShift = 0xa7
    }
}