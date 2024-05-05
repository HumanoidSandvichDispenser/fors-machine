using ForsMachine.Utils;
using ForsMachine.Assembler.Expressions;

namespace ForsMachine.Assembler.Instructions;

public enum LoadInstructionType
{
    Immediate,
    Register,
    Address,
    AddressInRegister,
}

public class LoadInstruction : Instruction
{
    public Register PRegister { get; set; }

    public Value Argument { get; set; }

    public bool IsArgumentAddress { get; set; }

    public LoadInstruction(
        Token<TokenType> source,
        Register pRegister,
        Value argument,
        bool isArgumentAddress) : base(source)
    {
        PRegister = pRegister;
        Argument = argument;
        IsArgumentAddress = isArgumentAddress;
    }

    public override uint ToInstruction(Dictionary<string, uint> symTable)
    {
        uint instruction = 0x00;
        ushort arg = 0x0000;

        if (PRegister is null)
        {
            throw new NullReferenceException("p-register is not given.");
        }
        IList<int> t = new List<int>();

        arg = (ushort)Argument.Evaluate(symTable);

        if (Argument is Constant immediate)
        {
            instruction = 0x01;
        }
        else if (Argument is Register qRegister)
        {
            instruction = 0x02;
            arg = (ushort)(arg << 8);
        }
        else
        {
            throw new ArgumentException("Argument given is invalid.");
        }

        if (IsArgumentAddress)
        {
            instruction += 2;
        }

        instruction = instruction << 8;
        instruction |= PRegister.Address;
        instruction = instruction << 16;
        instruction |= arg;
        return instruction;
    }
}
