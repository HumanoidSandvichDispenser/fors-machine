using ForsMachine.Utils;
using ForsMachine.Assembler.Expressions;

namespace ForsMachine.Assembler.Instructions;

public class JumpInstructionParser : InstructionParser<JumpInstruction>
{
    private JumpInstructionType _type;
    private AssemblyExpression? _to;
    private Register? _condition;
    private bool _isRelative;

    public JumpInstructionParser(
        Token<TokenType> source,
        JumpInstructionType type,
        Iterator<Token<TokenType>> it) : base(source, it)
    {
        _type = type;
    }

    public override JumpInstruction Parse()
    {
        _isRelative = _type.HasFlag(JumpInstructionType.Relative);

        ParseCondition();
        ParseTo();
        return BuildInstruction();
    }

    public void ParseCondition()
    {
        if (!_type.HasFlag(JumpInstructionType.Conditional))
        {
            return;
        }

        AssemblyExpression? next = ScanValue();

        if (next is null || next is not Register)
        {
            FailFromInvalidArguments("Expected condition register.", _source);
        }
        else
        {
            _condition = (Register)next;
        }
    }

    public void ParseTo()
    {
        AssemblyExpression? next = ScanValue();
        if (next is null)
        {
            FailFromInvalidArguments("Expected condition register.", _source);
        }
        else
        {
            if (_type.HasFlag(JumpInstructionType.Register))
            {
                if (next is not Register)
                {
                    FailFromInvalidArguments("Expected register.", next.Source);
                }
            }
            else
            {
                if (next is not Constant)
                {
                    if (next is Label)
                    {
                        if (_type.HasFlag(JumpInstructionType.Relative))
                        {
                            FailFromInvalidArguments("Expected constant value.",
                                next.Source);
                        }
                    }
                    else
                    {
                        FailFromInvalidArguments("Expected valid jump target.",
                            next.Source);
                    }
                }
            }

            _to = next;
        }
    }

    public override JumpInstruction BuildInstruction()
    {
        if (_to is null)
        {
            throw new NullReferenceException();
        }

        return new JumpInstruction(_source, _to, _condition, _isRelative);
    }
}
