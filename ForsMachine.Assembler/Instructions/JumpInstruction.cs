using ForsMachine.Utils;
using ForsMachine.Assembler.Expressions;

namespace ForsMachine.Assembler.Instructions;

[Flags]
public enum JumpInstructionType
{
    AbsoluteUnconditionalImmediate = 0,
    Conditional = 1,
    Relative = 2,
    Register = 4, // if not register then immediate
}

public class JumpInstruction : Instruction
{
    public AssemblyExpression To { get; set; }

    public bool IsRelative { get; set; }

    public Register? ConditionalRegister { get; set; }

    public JumpInstruction(
        Token<TokenType> source,
        AssemblyExpression to,
        Register? conditionalRegister,
        bool isRelative) : base(source)
    {
        To = to;
        ConditionalRegister = conditionalRegister;
        IsRelative = isRelative;
    }

    public override uint ToInstruction()
    {
        uint instruction = 0x50;
        ushort toAddress;

        if (To is Label label)
        {
            toAddress = label.Address;
        }
        else if (To is Constant c)
        {
            toAddress = (ushort)c.Value;
        }
        else if (To is Register q)
        {
            toAddress = q.Address;
            instruction |= 0b0010;
        }
        else
        {
            throw new Exception("Unknown jump to");
        }

        if (IsRelative)
        {
            instruction |= 0b0100;
        }

        if (ConditionalRegister is not null)
        {
            instruction |= 0b0001;
            instruction = instruction << 8;
            instruction |= ConditionalRegister.Address;
            instruction = instruction << 8;
            instruction |= toAddress;
            instruction = instruction << 8;
        }
        else
        {
            instruction = instruction << 16;
            instruction |= toAddress;
            instruction = instruction << 8;
        }

        return instruction;
    }
}
