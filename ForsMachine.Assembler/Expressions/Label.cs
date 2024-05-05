using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Label : Symbol
{
    public Label(Token<TokenType> source, string name) : base(source, name)
    {
        Name = name;
    }
}
