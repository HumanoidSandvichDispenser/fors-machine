namespace ForsMachine.Compiler.Procedures;

public abstract class BinaryOperation : Operation
{
    public BinaryOperation(string name, Types.Type lhs, Types.Type rhs, Types.Type returnType)
        : base(name, returnType, false)
    {
        LeftType = lhs;
        RightType = rhs;
        Parameters.Add("left", lhs);
        Parameters.Add("right", rhs);
    }

    public Types.Type LeftType { get; set; }

    public Types.Type RightType { get; set; }
}
