namespace ForsMachine.Utils;

public class Token<T> where T : Enum
{
    public T Type { get; set; }
    public string Value { get; set; }
    public int Line { get; set; }
    public int Column { get; set; }

    public Token(T type, string value, int line, int col)
    {
        Type = type;
        Value = value;
        Line = line;
        Column = col;
    }

    public bool ValueEquals(Token<T> token)
    {
        return Type.Equals(token.Type) && Value == token.Value;
    }

    public override string ToString()
    {
        return $"({Type.ToString()} \"{Value}\" @ {Line}:{Column})";
    }

    /*
    public override bool Equals(object? o)
    {
        return base.Equals(o);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Token<TEnum> left, Token<TEnum> right)
    {
        return left.Type.Equals(right.Type) && left.Value == right.Value;
    }

    public static bool operator !=(Token<TEnum> left, Token<TEnum> right) => !(left == right);
    */
}
