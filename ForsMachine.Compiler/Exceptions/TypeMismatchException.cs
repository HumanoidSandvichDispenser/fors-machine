namespace ForsMachine.Compiler.Exceptions;

public class TypeMismatchException : AbstractCompilerException
{
    public TypeMismatchException(Types.Type received, Types.Type expected, StackFrame? trace = null)
        : base($"Type mismatch: {received.Name} received, {expected.Name} expected", trace)
    {

    }
}
