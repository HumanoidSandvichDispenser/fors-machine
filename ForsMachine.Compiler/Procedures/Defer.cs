namespace ForsMachine.Compiler.Procedures;

public class Defer : Procedure
{
    public Expression Subprocedure { get; set; }

    public Defer(Expression subprocedure) : base("defer", subprocedure.Type, false)
    {
        Subprocedure = subprocedure;
    }

    private void ResolveTypes(StackFrame sf)
    {
        Type = Subprocedure.Type;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (stackFrame is null)
        {
            throw new NullReferenceException("StackFrame is null");
        }
               
        var asm = Subprocedure.GenerateAsm(stackFrame, shouldLoad);
        ResolveTypes(stackFrame);
        return asm;
    }
}
