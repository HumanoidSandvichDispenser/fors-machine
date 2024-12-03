namespace ForsMachine.Compiler.Exceptions;

public class AttributeNotFoundException : Exception
{
    public AttributeNotFoundException(string attribute, Types.Type type)
        : base($"Attribute {attribute} not found in type {type.Name}")
    {

    }
}
