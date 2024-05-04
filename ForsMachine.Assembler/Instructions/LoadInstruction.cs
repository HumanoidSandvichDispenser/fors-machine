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

    public AssemblyExpression Argument { get; set; }

    public bool IsArgumentAddress { get; set; }

    public LoadInstruction(
        Token<TokenType> source,
        Register pRegister,
        AssemblyExpression argument,
        bool isArgumentAddress) : base(source)
    {
        PRegister = pRegister;
        Argument = argument;
        IsArgumentAddress = isArgumentAddress;
    }

    public override uint ToInstruction()
    {
        uint instruction = 0x00;
        ushort arg = 0x0000;

        if (PRegister is null)
        {
            throw new NullReferenceException("p-register is not given.");
        }
        IList<int> t = new List<int>();

        if (Argument is Constant immediate)
        {
            instruction = 0x01;
            arg = (ushort)immediate.Value;
        }
        else if (Argument is Register qRegister)
        {
            // 0x02 p q 0x00
            instruction = 0x02;
            arg = qRegister.Address;
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
