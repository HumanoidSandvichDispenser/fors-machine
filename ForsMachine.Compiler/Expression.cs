namespace ForsMachine.Compiler;

public abstract class Expression
{
    public virtual Types.Type Type { get; set; }

    public Expression(Types.Type type)
    {
        Type = type;
    }

    //public Expression()
    //{
    //    if (Type is null)
    //    {
    //        throw new Exception("Expression.Type must be set");
    //    }
    //}

    /// <summary>
    /// Generate assembly code for this expression, with evaluated values in
    /// rax if shouldLoad is true.
    /// </summary>
    public abstract string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false);
}
