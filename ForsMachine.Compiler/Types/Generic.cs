namespace ForsMachine.Compiler.Types;

public abstract class Generic : Types.Type
{
    public Type PointerOf { get; set; }

    public static Dictionary<Type, Pointer> PointerTypes = new();

    public static GetPointerOfType(Type type)
    {
        return new Pointer { PointerOf = type };
    }

    public Pointer() : base("pointer", 1)
    {

    }
}
