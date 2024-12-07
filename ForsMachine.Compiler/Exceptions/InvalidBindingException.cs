namespace ForsMachine.Compiler.Exceptions;

public class InvalidBindingException : AbstractCompilerException
{
    public InvalidBindingException(string name, StackFrame? trace) :
        base($"The name \"{name}\" does not exist in the current context.", trace)
    {

    }
}
