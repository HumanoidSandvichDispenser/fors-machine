namespace ForsMachine.Compiler.Procedures;

public class ArithmeticOperation : BinaryOperation
{
    public static ArithmeticOperation Add { get; private set; }

    public static ArithmeticOperation Subtract { get; private set; }

    public static ArithmeticOperation Multiply { get; private set; }

    public static ArithmeticOperation Divide { get; private set; }

    private string _instruction;

    static ArithmeticOperation()
    {
        Add = new("add", "add");
        Subtract = new("subtract", "sub");
        Multiply = new("multiply", "mul");
        Divide = new("divide", "div");
    }

    private ArithmeticOperation(string name, string instruction)
        : base(
            name,
            Types.TypeSystem.Types["int16"],
            Types.TypeSystem.Types["int16"],
            Types.TypeSystem.Types["int16"]
        )
    {
        _instruction = instruction;
    }

    public override string[] GenerateInvocation(StackFrame stackFrame)
    {
        return [
            ..base.GenerateInvocation(stackFrame),
            "pop r0",
            "pop r1",
            $"{_instruction} rax, r0, r1",
        ];
    }
}
