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
    public Value To { get; set; }

    public bool IsRelative { get; set; }

    public Register? ConditionalRegister { get; set; }

    public JumpInstruction(
        Token<TokenType> source,
        Value to,
        Register? conditionalRegister,
        bool isRelative) : base(source)
    {
        To = to;
        ConditionalRegister = conditionalRegister;
        IsRelative = isRelative;
    }

    public override uint ToInstruction(Dictionary<string, uint> symTable)
    {
        uint instruction = 0x50;
        uint toAddress = To.Evaluate(symTable);

        if (To is Register q)
        {
            instruction |= 0b0010;
        }

        if (IsRelative)
        {
            instruction |= 0b0100;
        }

        if (ConditionalRegister is not null)
        {
            instruction |= 0b0001;
            var pAddress = ConditionalRegister.Evaluate(symTable);
            instruction = instruction.Insert(pAddress, 8);
        }
        else
        {
            instruction = instruction.Insert(0, 8);
        }

        instruction = instruction.Insert(toAddress, 16);

        return instruction;
    }
}
