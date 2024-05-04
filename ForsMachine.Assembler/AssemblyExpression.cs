using ForsMachine.Utils;

namespace ForsMachine.Assembler;

public abstract class AssemblyExpression : IParsableExpression
{
    public Token<TokenType> Source { get; set; }

    public AssemblyExpression(Token<TokenType> source)
    {
        Source = source;
    }
}
