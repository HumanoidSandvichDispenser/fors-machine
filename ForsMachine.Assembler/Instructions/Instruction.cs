using ForsMachine.Utils;

namespace ForsMachine.Assembler.Instructions;

public abstract class Instruction : AssemblyExpression
{
    public ushort Address { get; set; }

    public Instruction(Token<TokenType> source) : base(source)
    {

    }

    public abstract uint ToInstruction(Dictionary<string, uint> symbolTable);
}
