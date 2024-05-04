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

    public override uint ToInstruction()
    {
        uint instruction = 0x10;
        if (_argument is Constant m)
        {
            instruction = 0x10;
            if (m.Value >= 0x100)
            {
                System.Console.Error.WriteLine(
                    "WARN: Can not store an immediate value >= 0x100.");
            }
            m.Value &= 0x11; // limit to 1 byte
            instruction = instruction << 8;
            instruction |= (ushort)m.Value;
        }
        else if (_argument is Register p)
        {
            instruction = 0x11;
            instruction = instruction << 8;
            instruction |= p.Address;
        }

        instruction = instruction << 16;
        instruction |= (ushort)_address.Value;
        return instruction;
    }
}
