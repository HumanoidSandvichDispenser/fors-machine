using ForsMachine.Compiler.Types;

namespace ForsMachine.Compiler;

/// <summary>
/// Atomic value of a primitive type, holding a single addressable value.
/// </summary>
public class Atom : Expression
{
    public Types.PrimitiveType PrimitiveType { get; set; }

    public override Types.Type Type
    {
        get => PrimitiveType;
        set => PrimitiveType = (Types.PrimitiveType)value;
    }

    public ushort Value { get; set; }

    public Atom(ushort value)
        : this(value, TypeSystem.Types.GetPrimitiveType("int16")) { }

    public Atom(ushort value, Types.PrimitiveType type) : base(type)
    {
        PrimitiveType = type;
        Value = value;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (!shouldLoad)
        {
            return [];
        }

        return [
            "load rax, " + Value
        ];
    }

    public bool CanStoreImmediate()
    {
        return Value <= 0b0111_1111;
    }
}
