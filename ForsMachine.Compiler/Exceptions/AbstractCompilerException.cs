using System;

namespace ForsMachine.Compiler.Exceptions;

public class AbstractCompilerException : Exception
{
    public StackFrame? Trace { get; set; }

    public AbstractCompilerException(string message, StackFrame? trace = null)
        : base(message)
    {
        Trace = trace;
        this.Data.Add("Trace", trace);
    }
}
