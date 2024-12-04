using System;

namespace ForsMachine.Compiler.Exceptions;

public class ArgumentCountMismatchException : AbstractCompilerException
{
    public ArgumentCountMismatchException(
        string procedureName,
        int expected,
        int actual,
        StackFrame? trace = null
    )
        : base(
            $"Proceudre \"{procedureName}\" expected {expected} arguments, but got {actual}.",
            trace
        ) { }
}
