namespace ForsMachine.Compiler.Exceptions;

public class TypeNotPrimitiveException : Exception
{
    public TypeNotPrimitiveException(Types.Type type)
        : base($"Type {type.Name} is not a primitive type.") { }
}
