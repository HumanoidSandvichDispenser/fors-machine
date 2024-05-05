using ForsMachine.Utils;
using ForsMachine.Assembler.Expressions;
using ForsMachine.Assembler.Instructions;

namespace ForsMachine.Assembler;

public class AssemblyParser : Parser<AssemblyExpression, TokenType>
{
    protected HashSet<TokenType> _endTokens = new HashSet<TokenType>
    {
        TokenType.End,
    };

    public AssemblyParser(Iterator<Token<TokenType>> it) : base(it)
    {

    }

    protected override bool IsEnd(Token<TokenType>? token)
    {
        if (token is null)
        {
            var prevToken = _iterator.GetNext(-2);
            throw new InterpreterException("Unexpected EOF.",
                prevToken?.Line ?? 0, prevToken?.Column ?? 0);
        }

        return _endTokens.Contains(token.Type);
    }

    public override AssemblyExpression? NextExpression(AssemblyExpression? prev)
    {
        foreach (var end in _endTokens)
        {
            if (end == _iterator.GetNext()?.Type)
            {
                return prev;
            }
        }

        var token = _iterator.MoveNext();

        if (prev is Symbol symbol && token is not null)
        {
            if (token.Type == TokenType.LabelDefinition)
            {
                return new Label(symbol.Source, symbol.Name);
            }
            else
            {
                throw new InterpreterException("Unknown instruction \"" +
                    token.Type + "\".", token.Line, token.Column);
            }
        }

        if (prev is null && token?.Type == TokenType.Identifier)
        {
            var instruction = MapInstruction(token);
            if (instruction is not null)
            {
                return instruction;
            }
            else
            {
                return NextExpression(new Symbol(token, token.Value));
            }
        }

        throw new InterpreterException($"Unexpected token '{token?.Value}'.",
            token?.Line ?? 0, token?.Column ?? 0);
    }

    public Value? ScanValue()
    {
        var token = _iterator.GetNext();

        if (IsEnd(token))
        {
            return null;
        }

        _iterator.MoveNext();

        switch (token?.Type)
        {
            case TokenType.Register:
                {
                    var next = ScanValue();
                    if (next is Constant c)
                    {
                        return new Register(token, (byte)c.Value);
                    }
                    else if (next is null)
                    {
                        throw new InterpreterException(
                            "Expected register address.",
                            token.Line, token.Column);
                    }
                    throw new InterpreterException(next.ToString() +
                        " is not a valid register address.",
                        token.Line, token.Column);
                }
            case TokenType.Identifier:
                {
                    return new Symbol(token, token.Value);
                }
            case TokenType.Label:
                {
                    return new Label(token, token.Value);
                }
            case TokenType.Number:
                {
                    int numberBase = 10;
                    if (token.Value.Length > 2)
                    {
                        switch (token.Value[1])
                        {
                            case 'x':
                                numberBase = 16;
                                break;
                            case 'b':
                                numberBase = 2;
                                break;
                        }
                    }
                    var value = Convert.ToInt16(token.Value, numberBase);
                    return new Constant(token, value);
                }
            default:
                throw new InterpreterException("Invalid token.",
                    token?.Line ?? -1, token?.Column ?? -1);
        }
    }

    public Instruction? MapInstruction(Token<TokenType> token)
    {
        switch (token.Value)
        {
            /** LOAD opcodes */
            case "load":
            case "li":
                return new LoadInstructionParser(token,
                    LoadInstructionType.Immediate,
                    _iterator).Parse();
            case "load-q":
            case "lq":
                return new LoadInstructionParser(token,
                    LoadInstructionType.Register,
                    _iterator).Parse();
            case "load-addr":
            case "la":
                return new LoadInstructionParser(token,
                    LoadInstructionType.Address,
                    _iterator).Parse();
            case "load-q-addr":
            case "lqa":
                return new LoadInstructionParser(token,
                    LoadInstructionType.AddressInRegister,
                    _iterator).Parse();
            /** STORE opcodes */
            case "store":
            case "si":
                return new StoreInstructionParser(token,
                    StoreInstructionType.Immediate,
                    _iterator).Parse();
            case "store-p":
            case "sp":
                return new StoreInstructionParser(token,
                    StoreInstructionType.Register,
                    _iterator).Parse();
            /** JUMP opcodes */
            case "jump":
            case "jmp":
                return new JumpInstructionParser(token,
                    JumpInstructionType.AbsoluteUnconditionalImmediate,
                    _iterator).Parse();
            case "jump-if-p":
            case "jmpp":
                return new JumpInstructionParser(token,
                    JumpInstructionType.Conditional,
                    _iterator).Parse();
            case "jump-q":
            case "jmpq":
                return new JumpInstructionParser(token,
                    JumpInstructionType.Register,
                    _iterator).Parse();
            case "jump-q-if-p":
            case "jmppq":
                return new JumpInstructionParser(token,
                    JumpInstructionType.Register |
                    JumpInstructionType.Conditional,
                    _iterator).Parse();
            default:
                return null;
        }
    }
}
