namespace ForsMachine.Compiler.Exceptions;

public class TypeNotFoundException : Exception
{
    public TypeNotFoundException(string type) : base($"Type '{type}' not found.")
    {

    }
}
