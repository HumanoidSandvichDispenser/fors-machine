using ForsMachine.Utils;

namespace ForsMachine.Assembler.Instructions;

public abstract class InstructionParser<T> : AssemblyParser
{
    protected Token<TokenType> _source;

    public InstructionParser(
        Token<TokenType> source,
        Iterator<Token<TokenType>> it) : base(it)
    {
        _source = source;
    }

    public abstract new T Parse();

    public abstract T BuildInstruction();

    public void FailFromInvalidArguments(string msg, Token<TokenType> source)
    {
        throw new InterpreterException(msg, source.Line, source.Column);
    }
}
