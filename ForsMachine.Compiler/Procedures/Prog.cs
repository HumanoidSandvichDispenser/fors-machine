namespace ForsMachine.Compiler.Procedures;

/// <summary>
/// A series of instructions evaluated in order, returning the value of the
/// last instruction. This also creates a new scope for its instructions.
/// </summary>
public class Prog : Operation
{
    public Prog(List<Expression> instructions)
        : base("prog", instructions.Last().Type, false)
    {
        Instructions = instructions;
    }

    private void ResolveType()
    {
        Type = Instructions.Last().Type;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (stackFrame is null)
        {
            throw new InvalidOperationException();
        }

        stackFrame.EnterScope();

        // defer goes at the end of the scope
        Instructions.OrderBy((a) => a is Defer ? 1 : 0);

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

        stackFrame.ExitScope();

        ResolveType();

        // last instruction is implicitly returned because every expression
        // sets rax to its evaluated value
        return asm.ToArray();
    }
}
