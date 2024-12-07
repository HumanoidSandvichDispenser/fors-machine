namespace ForsMachine.Compiler;

public class OuterExpression : Expression
{
    public Expression Subexpression { get; set; }

    public OuterExpression(Expression subexpression) : base(subexpression.Type)
    {
        Subexpression = subexpression;
    }

    public void ResolveType()
    {
        Type = Subexpression.Type;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (stackFrame is null)
        {
            throw new NullReferenceException("StackFrame is null");
        }

        var asm = Subexpression.GenerateAsm(stackFrame, shouldLoad);

        ResolveType();
        return asm;
    }
}
