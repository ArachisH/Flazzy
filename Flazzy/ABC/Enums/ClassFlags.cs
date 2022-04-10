namespace Flazzy.ABC;

[Flags]
public enum ClassFlags
{
    /// <summary>
    /// Represents no flags for this class.
    /// </summary>
    None = 0,
    /// <summary>
    /// Represents a sealed class where properties can't be dynamically added to instances of the class.
    /// </summary>
    Sealed = 1 << 0,
    /// <summary>
    /// Represents a class that can't be used as a base class for any other class.
    /// </summary>
    Final = 1 << 1,
    /// <summary>
    /// Represents a class that is of interface type
    /// </summary>
    Interface = 1 << 2,
    /// <summary>
    /// Represents a class that uses its' protected namespace meaning the property <see cref="ASInstance.ProtectedNamespace"/> is present.
    /// </summary>
    ProtectedNamespace = 1 << 3
}
