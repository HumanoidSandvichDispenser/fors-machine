using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public abstract class Value : AssemblyExpression
{
    public Value(Token<TokenType> source) : base(source)
    {

    }

    public abstract uint Evaluate(Dictionary<string, uint> symbolTable);
}
