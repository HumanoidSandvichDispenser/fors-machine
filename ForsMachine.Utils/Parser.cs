namespace ForsMachine.Utils;

public abstract class Parser<TExpression, TToken>
    where TExpression : class?
    where TToken : Enum
{
    protected Iterator<Token<TToken>> _iterator;

    public Parser(Iterator<Token<TToken>> iterator)
    {
        _iterator = iterator;
    }

    public abstract TExpression? NextExpression(TExpression? prev);

    public IEnumerable<TExpression> Parse()
    {
        while (_iterator.GetNext() != null)
        {
            var expr = NextExpression(null);
            if (expr is not null)
            {
                yield return expr;
            }
            _iterator.MoveNext();
        }
    }

    public void FailIfAtEOF(Token<TToken>? token)
    {
        if (token is null)
        {
            var prevToken = _iterator.GetNext(-2);
            throw new InterpreterException("Unexpectd EOF.",
                prevToken?.Line ?? 0, prevToken?.Column ?? 0);
        }
    }

    protected abstract bool IsEnd(Token<TToken>? token);
}
