using ForsMachine.Utils;
using ForsMachine.Compiler.Frontend;

namespace ForsMachine.Compiler.Exceptions;

public class SyntaxException : Exception
{
    public int Line { get; set; }

    public int Column { get; set; }

    public SyntaxException(string message, Token<TokenType>? token)
        : base($"{message} ({token?.Line}:{token?.Column})")
    {
        Line = token?.Line ?? -1;

        Column = token?.Line ?? -1;
    }
}
