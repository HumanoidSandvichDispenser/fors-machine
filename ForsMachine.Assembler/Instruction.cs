namespace ForsMachine.Assembler;

public abstract class Instruction : AssemblyExpression
{
    public abstract uint ToInstruction();
}
