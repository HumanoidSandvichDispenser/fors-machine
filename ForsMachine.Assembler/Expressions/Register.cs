using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Register : AssemblyExpression, ILoadArgument
{
    public byte Address { get; set; }

    public Register(Token<TokenType> source, byte address) : base(source)
    {
        Address = address;
    }
}
