using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Constant : Value, ILoadArgument
{
    public short Value { get; set; }

    public Constant(Token<TokenType> source, short value) : base(source)
    {
        Value = value;
    }

    public override uint Evaluate(Dictionary<string, uint> _) => (uint)Value;
}
