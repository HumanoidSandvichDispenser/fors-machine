namespace ForsMachine.Assembler;

public class Constant : AssemblyExpression
{
    public short Value { get; set; }

    public Constant(short value)
    {
        Value = value;
    }
}
