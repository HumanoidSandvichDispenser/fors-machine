namespace ForsMachine.Compiler.Types;

public abstract class PrimitiveType : Type
{
    private int _size = 1;

    public override int Size { get => _size; protected set => _size = value; }

    public PrimitiveType(string name, int size) : base(name)
    {
        Name = name;
        Size = size;
    }
}
