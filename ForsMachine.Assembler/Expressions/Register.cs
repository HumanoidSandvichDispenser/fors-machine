using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Register : Value, ILoadArgument
{
    public byte Address { get; set; }

    public Register(Token<TokenType> source, byte address) : base(source)
    {
        Address = address;
    }

    public override uint Evaluate(Dictionary<string, uint> _) => Address;
}
