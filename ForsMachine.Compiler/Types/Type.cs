namespace ForsMachine.Compiler.Types;

public abstract class Type
{
    public string Name { get; set; }

    /// <summary>
    /// Size in addressable units
    /// </summary>
    public abstract int Size { get; protected set; }

    public Type(string name)
    {
        Name = name;
    }
}
