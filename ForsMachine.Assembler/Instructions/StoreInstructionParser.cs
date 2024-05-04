using ForsMachine.Utils;
using ForsMachine.Assembler.Expressions;

namespace ForsMachine.Assembler.Instructions;

public class StoreInstructionParser : InstructionParser<StoreInstruction>
{
    private StoreInstructionType _type;
    private AssemblyExpression? _argument;
    private Constant? _address;

    public StoreInstructionParser(
        Token<TokenType> source,
        StoreInstructionType type,
        Iterator<Token<TokenType>> it) : base(source, it)
    {
        _type = type;
    }

    public override StoreInstruction Parse()
    {
        ParseArgument();
        ParseAddress();
        return BuildInstruction();
    }

    public void ParseArgument()
    {
        AssemblyExpression? next = ScanValue();

        if (next is null)
        {
            FailFromInvalidArguments("Argument was not given.", _source);
        }
        else
        {
            switch (_type)
            {
                case StoreInstructionType.Register:
                    if (next is not Register)
                    {
                        FailFromInvalidArguments("Expected register.",
                            next.Source);
                    }
                    break;
                case StoreInstructionType.Immediate:
                    if (next is not Constant)
                    {
                        FailFromInvalidArguments("Expected constant value.",
                            next.Source);
                    }
                    break;
            }
        }
        _argument = next;
    }

    public void ParseAddress()
    {
        AssemblyExpression? next = ScanValue();

        if (next is null)
        {
            FailFromInvalidArguments("p register was not given.", _source);
        }

        if (next is Constant addr)
        {
            _address = addr;
        }
        else
        {
            throw new InterpreterException($"Invalid constant value.",
                next?.Source.Line ?? 0, next?.Source.Column ?? 0);
        }
    }

    public override StoreInstruction BuildInstruction()
    {
        if (_address is null || _argument is null)
        {
            throw new NullReferenceException();
        }

        return new StoreInstruction(_argument, _address, _source);
    }
}
