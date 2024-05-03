namespace ForsMachine.Assembler;

public class Label : AssemblyExpression
{
    public string Name { get; set; }

    public ushort Address
    {
        get
        {
            if (!IsResolved)
            {
                throw new UnresolvedLabelException(
                    $"Tried to get the address of label {Name} " +
                    "without first resolving it.");
            }

            return _address;
        }
    }

    private ushort _address;

    public bool IsResolved { get; private set; }

    public Instruction? NextInstruction { get; set; }

    public Label(string name, Instruction? next)
    {
        Name = name;
        NextInstruction = next;
    }

    public void ResolveAddress(ushort address)
    {
        IsResolved = true;
        _address = address;
    }
}
