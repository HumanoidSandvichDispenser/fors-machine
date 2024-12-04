namespace ForsMachine.Compiler.Exceptions;

public class InvalidBindingException : Exception
{
    public InvalidBindingException(string name) :
        base($"The name \"{name}\" does not exist in the current context.")
    {

    }
}
