namespace ForsMachine.Utils;

public abstract class Parser<TExpression, TToken>
    where TExpression : class?
    where TToken : Enum
{
    protected Iterator<Token<TToken>>? _iterator;

    public abstract TExpression? NextExpression(TExpression? prev);

    public static IEnumerable<TExpression> Parse<T>(
        Token<TToken>[] tokens, Func<Token<TToken>[], T> creator)
        where T : Parser<TExpression, TToken>
    {
        T parser = creator(tokens);

        var iterator = parser._iterator;

        while (iterator?.GetNext() != null)
        {
            var expr = parser.NextExpression(null);
            if (expr is not null)
            {
                yield return expr;
            }
            iterator.MoveNext();
        }
    }
}
