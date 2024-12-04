namespace ForsMachine.Compiler.Procedures;

public class Conditional : Procedure
{
    public Expression Condition { get; set; }

    public Expression OnTrue { get; set; }

    public Expression? OnFalse { get; set; }

    public Conditional(Expression condition, Expression onTrue, Expression? onFalse)
        : base("conditional", onTrue.Type, false)
    {
        if (condition.Type is not Types.PrimitiveType)
        {
            throw new Exceptions.TypeNotPrimitiveException(condition.Type);
        }

        if (onFalse is not null && onTrue.Type != onFalse.Type)
        {
            throw new Exceptions.TypeMismatchException(onTrue.Type, onFalse.Type);
        }

        Condition = condition;
        OnTrue = onTrue;
        OnFalse = onFalse;
    }

    private IEnumerable<string> GenerateBody(StackFrame? stackFrame, bool shouldLoad)
    {
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

        return asm;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        return GenerateBody(stackFrame, shouldLoad).ToArray();
    }
}
