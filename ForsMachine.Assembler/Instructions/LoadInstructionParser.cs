using ForsMachine.Utils;
using ForsMachine.Assembler.Expressions;

namespace ForsMachine.Assembler.Instructions;

public class LoadInstructionParser : InstructionParser<LoadInstruction>
{
    private LoadInstructionType _type;
    private Register? _pRegister;
    private AssemblyExpression? _argument;

    public LoadInstructionParser(
        Token<TokenType> source,
        LoadInstructionType type,
        Iterator<Token<TokenType>> it) : base(source, it)
    {
        _type = type;
    }

    public override LoadInstruction Parse()
    {
        ParsePRegister();
        ParseArgument();
        return BuildInstruction();
    }

    public void ParsePRegister()
    {
        AssemblyExpression? next = ScanValue();
        if (next is null)
        {
            FailFromInvalidArguments("p register was not given.", _source);
        }

        if (next is Register p)
        {
            _pRegister = p;
        }
        else
        {
            throw new InterpreterException($"{next} is not a register.",
                next?.Source.Line ?? 0, next?.Source.Column ?? 0);
        }
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
                case LoadInstructionType.Address:
                case LoadInstructionType.Immediate:
                    if (next is not Constant)
                    {
                        FailFromInvalidArguments( "Expected constant value.",
                            next.Source);
                    }
                    break;
                case LoadInstructionType.Register:
                case LoadInstructionType.AddressInRegister:
                    if (next is not Register)
                    {
                        FailFromInvalidArguments("Expected register.",
                            next.Source);
                    }
                    break;
            }
        }
        _argument = next;
    }

    public override LoadInstruction BuildInstruction()
    {
        if (_pRegister is null || _argument is null)
        {
            throw new NullReferenceException();
        }

        bool isArgAddr = _type == LoadInstructionType.Address ||
            _type == LoadInstructionType.AddressInRegister;

        return new LoadInstruction(_source, _pRegister, _argument, isArgAddr);
    }
}
