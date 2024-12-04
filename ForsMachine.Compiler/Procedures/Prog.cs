namespace ForsMachine.Compiler.Procedures;

/// <summary>
/// A series of instructions evaluated in order, returning the value of the
/// last instruction.
/// </summary>
public class Prog : Operation
{
    public Prog(List<Expression> instructions)
        : base("prog", instructions.Last().Type, false)
    {
        Instructions = instructions;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        List<string> asm = [ "; prog" ];
        for (int i = 0; i < Instructions.Count; i++)
        {
            if (i == Instructions.Count - 1)
            {
                asm.AddRange(Instructions[i].GenerateAsm(stackFrame, shouldLoad));
            }
            else
            {
                asm.AddRange(Instructions[i].GenerateAsm(stackFrame));
            }
        }
        // last instruction is implicitly returned because every expression
        // sets rax to its evaluated value
        return asm.ToArray();
    }
}
