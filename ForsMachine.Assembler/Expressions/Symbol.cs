using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Symbol : AssemblyExpression
{
    public string Name { get; set; }

    public Symbol(Token<TokenType> source, string name) : base(source)
    {
        Name = name;
    }
}
