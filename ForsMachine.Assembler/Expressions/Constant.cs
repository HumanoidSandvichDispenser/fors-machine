using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Constant : AssemblyExpression, ILoadArgument
{
    public short Value { get; set; }

    public Constant(Token<TokenType> source, short value) : base(source)
    {
        Value = value;
    }
}
