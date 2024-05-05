using ForsMachine.Assembler.Expressions;
using ForsMachine.Utils;

namespace ForsMachine.Assembler.Instructions;

public enum StoreInstructionType
{
    Immediate,
    Register,
}

public class StoreInstruction : Instruction
{
    private AssemblyExpression _argument;

    private Constant _address;

    public StoreInstruction(
        AssemblyExpression argument,
        Constant address,
        Token<TokenType> source) : base(source)
    {
        _argument = argument;
        _address = address;
    }

    public override uint ToInstruction(Dictionary<string, uint> symTable)
    {
        uint instruction = 0x10;
        if (_argument is Constant m)
        {
            instruction = 0x10;
            uint value = m.Evaluate(symTable);
            if (value >= 0x100)
            {
                System.Console.Error.WriteLine(
                    "WARN: Can not store an immediate value >= 0x100.");
            }
            instruction = instruction.Insert(value, 8);
        }
        else if (_argument is Register p)
        {
            instruction = 0x11;
            instruction = instruction.Insert(p.Evaluate(symTable), 8);
        }

        return instruction.Insert(_address.Evaluate(symTable), 16);
    }
}
