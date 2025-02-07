using ForsMachine.Utils;

namespace ForsMachine.Compiler.Frontend;

public class Parser
{
    public Iterator<Token<TokenType>> Tokens { get; set; }

    private Token<TokenType>? _currentToken;

    public Parser(Iterator<Token<TokenType>> tokens)
    {
        Tokens = tokens;
        _currentToken = Consume();
    }

    public Token<TokenType>? Consume()
    {
        var token = Tokens.MoveNext();
        if (token is null && _currentToken?.Type != TokenType.End)
        {
            throw new Exceptions.SyntaxException("Unexpected end of input.", _currentToken);
        }
        _currentToken = token;
        return token;
    }

    // program -> statement*
    // statement -> expression ';'
    public IEnumerable<Expression> ParseProgram()
    {
        while (_currentToken is not null)
        {
            yield return ParseStatement();
        }
    }

    public Expression ParseStatement()
    {
        var expr = ParseExpression();
        Expect(TokenType.End);
        return expr;
    }

    public Expression? NextExpression(Expression? prev)
    {
        Expression? expression = null;

        if (_currentToken?.Type == TokenType.End)
        {
            return null;
        }

        if (_currentToken?.Type == TokenType.Grouping)
        {
            switch (_currentToken.Value)
            {
                case ")":
                case "]":
                case "}":
                    return null;
            }
        }

        if (_currentToken?.Type == TokenType.Operator)
        {
            if (prev is not null)
            {
                expression = ParseBinaryOperation(prev, _currentToken);
                return expression;
            }

            throw new Exceptions.SyntaxException("Unexpected token.", _currentToken);
        }
        else if (_currentToken?.Type == TokenType.Keyword)
        {
            if (prev is not null)
            {
                return null;
            }

            switch (_currentToken.Value)
            {
                case "function":
                    expression = ParseFunction();
                    break;
                case "with":
                    expression = ParseLet();
                    break;
                case "if":
                    expression = ParseIf();
                    break;
                case "defer":
                    Consume();
                    expression = new Procedures.Defer(ParseExpression());
                    break;
                case "mutate":
                    Consume();
                    return Procedures.MutationOperation.Mutate;
                default:
                    var msg = $"Keyword '{_currentToken.Value}' is not valid in this context.";
                    throw new Exceptions.SyntaxException(msg, _currentToken);
            }
        }
        else if (_currentToken?.Type == TokenType.Number)
        {
            if (prev is not null)
            {
                return null;
            }

            expression = new Atom(UInt16.Parse(_currentToken.Value));
            Consume();
        }
        else if (_currentToken?.Type == TokenType.Identifier)
        {
            if (prev is not null)
            {
                return null;
            }

            // this is a valid expression, but we need to check if it's a function call
            // or an operation

            var symbol = new Symbol(_currentToken.Value);
            expression = symbol;
            Consume();

            //if (SoftExpect(TokenType.Grouping, "(") is not null)
            //{
            //    var arguments = ParseArguments();
            //    expression = new Procedures.ProcedureInvocation(symbol, arguments.ToList());
            //}
        }
        else if (_currentToken?.Type == TokenType.Grouping && _currentToken.Value == "(")
        {
            if (prev is not null)
            {
                var arguments = ParseArguments();
                expression = new Procedures.ProcedureInvocation(prev, arguments.ToList());
                return expression;
            }
            else
            {
                Consume();
                expression = new OuterExpression(ParseExpression());
                Expect(TokenType.Grouping, ")");
                return expression;
            }
        }
        else if (_currentToken?.Type == TokenType.Grouping && _currentToken.Value == "{")
        {
            if (prev is not null)
            {
                return null;
            }

            Consume();
            // parse block as list of statements
            var statements = new List<Expression>();
            while (SoftExpect(TokenType.Grouping, "}") is null)
            {
                statements.Add(ParseStatement());
            }

            expression = new Procedures.Prog(statements);
        }

        return expression;
    }

    public Expression ParseExpression()
    {
        Expression? prev = null;
        Expression? fullExpression = null;

        while (true)
        {
            prev = NextExpression(prev);
            if (prev is null)
            {
                break;
            }
            fullExpression = prev;
        }

        if (fullExpression is null)
        {
            throw new Exceptions.SyntaxException("Expected expression.", _currentToken);
        }

        return fullExpression;
    }

    public Procedures.ProcedureInvocation ParseBinaryOperation(Expression? left, Token<TokenType> op)
    {
        if (left is null)
        {
            throw new Exceptions.SyntaxException("Expected left operand.", _currentToken);
        }

        Consume();
        var right = ParseExpression();
        Procedures.BinaryOperation binaryOperation;

        switch (op.Value)
        {
            case "+":
                binaryOperation = Procedures.ArithmeticOperation.Add;
                break;
            case "-":
                binaryOperation = Procedures.ArithmeticOperation.Subtract;
                break;
            case "*":
                binaryOperation = Procedures.ArithmeticOperation.Multiply;
                break;
            case "/":
                binaryOperation = Procedures.ArithmeticOperation.Divide;
                break;
            case "==":
                binaryOperation = Procedures.ComparisonOperation.Equal;
                break;
            default:
                throw new NotImplementedException();
        }

        var operation = new Procedures.ProcedureInvocation(binaryOperation, [left, right]);

        if (op.Value == "*" || op.Value == "/")
        {
            if (left is Procedures.ProcedureInvocation i)
            {
                if (i.Procedure is Procedures.ArithmeticOperation a)
                {
                    if (a == Procedures.ArithmeticOperation.Add ||
                        a == Procedures.ArithmeticOperation.Subtract)
                    {
                        operation.Arguments[0] = i.Arguments[1];
                        operation.Arguments[1] = right;
                    }
                }
            }
        }

        return operation;
    }

    public IEnumerable<Expression> ParseArguments()
    {
        Expect(TokenType.Grouping, "(");

        int i = 0;
        while (SoftExpect(TokenType.Grouping, ")") is null)
        {
            if (i++ > 0)
            {
                Expect(TokenType.Punctuation, ",");
            }
            yield return ParseExpression();
        }
    }

    public Procedures.Function ParseFunction()
    {
        Expect(TokenType.Keyword, "function");

        string? name = SoftExpect(TokenType.Identifier)?.Value;

        Expect(TokenType.Grouping, "(");
        BindingList parameters = ParseBindingList();
        Expect(TokenType.Grouping, ")");

        string returnType = ParseTypeAnnotation();

        Procedures.Function function = new(name, parameters, returnType);

        Expect(TokenType.Operator, "=");

        var body = ParseExpression();
        if (body is Procedures.Prog progn)
        {
            function.Instructions = progn.Instructions;
        }
        else
        {
            function.Instructions.Add(body);
        }
        return function;
    }

    public Procedures.BindSubprocedure ParseLet()
    {
        Expect(TokenType.Keyword, "with");
        Expect(TokenType.Grouping, "(");
        var bindings = ParseBindingList(true);
        Expect(TokenType.Grouping, ")");
        var subprocedure = ParseExpression();

        return new(bindings, subprocedure);
    }

    public BindingList ParseBindingList(bool enforceDefaultValues = false)
    {
        var parameters = new BindingList();

        int i = 0;
        while (SoftExpect(TokenType.Grouping, ")", false) is null)
        {
            if (i++ > 0)
            {
                Expect(TokenType.Punctuation, ",");
            }

            var id = Expect(TokenType.Identifier);

            var type = ParseTypeAnnotation();
            Expression? defaultValue = null;

            if (SoftExpect(TokenType.Operator, "=") is not null)
            {
                defaultValue = ParseExpression();
                enforceDefaultValues = true;
            }
            else if (enforceDefaultValues)
            {
                throw new Exceptions.SyntaxException(
                    $"Expected {id} to be assigned a value.",
                    _currentToken
                );
            }

            parameters.Bindings.Add(id.Value, new(type, defaultValue));
        }

        return parameters;
    }

    public string ParseTypeAnnotation()
    {
        Expect(TokenType.TypeAnnotation);
        return Expect(TokenType.Identifier).Value;
    }

    public Procedures.Conditional ParseIf()
    {
        Expect(TokenType.Keyword, "if");

        Expect(TokenType.Grouping, "(");
        var condition = ParseExpression();
        Expect(TokenType.Grouping, ")");

        var ifBlock = ParseExpression();

        Expression? elseBlock = null;
        if (SoftExpect(TokenType.Keyword, "else") is not null)
        {
            elseBlock = ParseExpression();
        }

        return new(condition, ifBlock, elseBlock);
    }

    private Token<TokenType>? SoftExpect(TokenType expectedType, string? expectedValue = null, bool consume = true)
    {
        if (_currentToken?.Type != expectedType)
        {
            return null;
        }

        if (expectedValue != null && _currentToken?.Value != expectedValue)
        {
            return null;
        }

        var token = _currentToken;
        if (consume)
        {
            Consume();
        }
        return token;
    }

    private Token<TokenType> Expect(TokenType expectedType, string? expectedValue = null)
    {
        bool unexpectedType = _currentToken?.Type != expectedType;
        bool unexpectedValue = expectedValue != null && _currentToken?.Value != expectedValue;
        if (_currentToken == null || unexpectedType || unexpectedValue)
        {
            var msg =
                $"Expected {expectedType} with value '{expectedValue}',"
                + $" but found {_currentToken?.Type} {_currentToken?.Value}.";

            throw new Exceptions.SyntaxException(msg, _currentToken);
        }

        var token = _currentToken;
        Consume();
        return token;
    }
}
