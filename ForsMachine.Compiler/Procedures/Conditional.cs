namespace ForsMachine.Compiler.Procedures;

public class Conditional : Procedure
{
    public Expression Condition { get; set; }

    public Expression OnTrue { get; set; }

    public Expression? OnFalse { get; set; }

    public Conditional(Expression condition, Expression onTrue, Expression? onFalse)
        : base("conditional", onTrue.Type, false)
    {
        Condition = condition;
        OnTrue = onTrue;
        OnFalse = onFalse;
    }

    private void ResolveTypes(StackFrame sf)
    {
        if (Condition.Type is not Types.PrimitiveType)
        {
            throw new Exceptions.TypeNotPrimitiveException(Condition.Type);
        }

        if (OnFalse is not null && OnTrue.Type != OnFalse.Type)
        {
            throw new Exceptions.TypeMismatchException(OnTrue.Type, OnFalse.Type, sf);
        }

        Type = OnTrue.Type;
    }

    private IEnumerable<string> GenerateBody(StackFrame? stackFrame, bool shouldLoad)
    {
        if (stackFrame is null)
        {
            throw new NullReferenceException("StackFrame is null");
        }

        List<string> asm = new();

        string hash = GetHashCode().ToString();
        string trueLabel = $".conditional_{hash}";
        string endLabel = $".end_conditional_{hash}";

        var condAsm = Condition.GenerateAsm(stackFrame, true);
        asm.AddRange(condAsm);
        asm.Add($"jmp_if rax, &{trueLabel}");

        if (OnFalse is not null)
        {
            var falseAsm = OnFalse.GenerateAsm(stackFrame, shouldLoad);
            asm.Add("; else/false branch");
            asm.AddRange(falseAsm);
        }

        asm.Add($"jmp &{endLabel}");

        asm.Add($"{trueLabel}:");
        var trueAsm = OnTrue.GenerateAsm(stackFrame, shouldLoad)
            .Select(x => $"\t{x}");
        asm.AddRange(trueAsm);
        asm.Add($"{endLabel}:");

        ResolveTypes(stackFrame);

        return asm;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        return GenerateBody(stackFrame, shouldLoad).ToArray();
    }
}
