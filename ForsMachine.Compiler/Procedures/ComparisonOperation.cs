namespace ForsMachine.Compiler.Procedures;

public class ComparisonOperation : BinaryOperation
{
    public static ComparisonOperation Equal { get; private set; }

    public static ComparisonOperation NotEqual { get; private set; }

    public static ComparisonOperation GreaterThan { get; private set; }

    public static ComparisonOperation LessThan { get; private set; }

    public static ComparisonOperation GreaterThanOrEqual { get; private set; }

    public static ComparisonOperation LessThanOrEqual { get; private set; }

    private int _resultShift;

    static ComparisonOperation()
    {
        Equal = new("eq", 0);
        NotEqual = new("neq", 1);
        GreaterThan = new("gt", 2);
        LessThan = new("lt", 3);
        GreaterThanOrEqual = new("gte", 4);
        LessThanOrEqual = new("lte", 5);
    }

    private ComparisonOperation(string name, int resultShift)
        : base(
            name,
            Types.TypeSystem.Types["int16"],
            Types.TypeSystem.Types["int16"],
            Types.TypeSystem.Types["int16"]
        )
    {
        _resultShift = resultShift;
    }

    public override string[] GenerateInvocation(StackFrame stackFrame)
    {
        string shift = $"rshift rax, rax, {_resultShift}";
        List<string> instructions = [
            ..base.GenerateInvocation(stackFrame),
            "pop r0",
            "pop r1",
            "cmp rax, r0, r1",
        ];

        if (_resultShift != 0)
        {
            instructions.Add(shift);
        }

        return instructions.ToArray();
    }
}
