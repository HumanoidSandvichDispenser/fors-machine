namespace ForsMachine.Compiler.Procedures;

public class DereferenceOperation : UnaryOperation
{
    public Expression Operand { get; set; }

    private DereferenceOperation(Expression operand, string typeName) :
        base("dereference", Types.TypeSystem.Types["unknown"], Types.TypeSystem.Types["unknown"])
    {
        Operand = operand;
    }

    public override string[] GenerateInvocation(StackFrame stackFrame)
    {
        return [
            "pop r0",
            "load rax, &r0",
        ];
    }
}
