namespace ForsMachine.Compiler.Types;

public class TypeStruct : Type
{
    public override int Size
    {
        get => Attributes.Sum((attribute) => attribute.Value.Size);
        protected set => throw new NotImplementedException();
    }

    public SortedDictionary<string, Type> Attributes { get; set; }

    public int GetOffset(string name)
    {
        var offset = 0;
        foreach (var attribute in Attributes)
        {
            if (attribute.Key == name)
            {
                return offset;
            }
            offset += attribute.Value.Size;
        }
        throw new Exceptions.AttributeNotFoundException(name, this);
    }

    public TypeStruct(string name, SortedDictionary<string, Type> attributes) : base(name)
    {
        Attributes = attributes;
    }
}
