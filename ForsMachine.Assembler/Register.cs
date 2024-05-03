namespace ForsMachine.Assembler;

public class Register : AssemblyExpression
{
    public byte Address { get; set; }

    public Register(byte address)
    {
        Address = address;
    }
}
