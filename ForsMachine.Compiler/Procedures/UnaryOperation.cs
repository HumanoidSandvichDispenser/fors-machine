namespace ForsMachine.Compiler.Procedures;

public abstract class UnaryOperation : Operation
{
    public Types.Type OperandType { get; set; }

    public UnaryOperation(string name, Types.Type operandType, Types.Type returnType)
        : base(name, returnType, false)
    {
        OperandType = operandType;
        Parameters.Add("operand", OperandType);
    }
}
