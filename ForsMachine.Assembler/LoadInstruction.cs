namespace ForsMachine.Assembler;

public class LoadInstruction : Instruction
{
    public Register PRegister { get; set; }

    public AssemblyExpression Argument { get; set; }

    public bool IsArgumentAddress { get; set; }

    public LoadInstruction(Register pRegister,
        AssemblyExpression rhs, bool isArgumentAddress = false)
    {
        PRegister = pRegister;
        Argument = rhs;
        IsArgumentAddress = isArgumentAddress;
    }

    public override uint ToInstruction()
    {
        uint instruction = 0x00;
        ushort arg = 0x0000;

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
