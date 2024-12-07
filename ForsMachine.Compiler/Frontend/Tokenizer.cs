using System.Text.RegularExpressions;
using ForsMachine.Utils;

namespace ForsMachine.Compiler.Frontend;

public class Tokenizer : ForsMachine.Utils.Tokenizer<TokenType>
{
    protected static readonly Regex REGEX_IDENTIFIER_START = new Regex("[_a-zA-Z]");
    protected static readonly Regex REGEX_IDENTIFIER = new Regex("[_\\-a-zA-Z0-9]");

    public readonly HashSet<string> KEYWORDS = new HashSet<string>
    {
        "with",
        "if",
        "function",
        "operation",
        "else",
        "defer",
    };

    public readonly HashSet<char> GROUPING = new HashSet<char> { '(', ')', '{', '}', '[', ']', };

    public readonly HashSet<char> DELIMITERS = new HashSet<char> { ',' };

    public readonly HashSet<string> OPERATORS = new HashSet<string>
    {
        "+",
        "-",
        "*",
        "/",
        "%",
        "=",
        "==",
        "!=",
        ">",
        "<",
        ">=",
        "<=",
        "&",
        "|",
        "^",
        "~",
    };

    public HashSet<string> PotentialOperators(string initial)
    {
        return OPERATORS.Where((op) => op.StartsWith(initial)).ToHashSet();
    }

    public readonly char TYPE_ANNOTATION = ':';

    public readonly char END = ';';

    public override IEnumerable<Token<TokenType>> Lex(CharIterator iterator)
    {
        int line,
            col;
        while (iterator.GetNext() != default)
        {
            char c = iterator.MoveNext();
            line = iterator.Line;
            col = iterator.Column;

            if (c == '#')
            {
                while (true)
                {
                    char next = iterator.GetNext();
                    if (next == '\n' || next == default)
                    {
                        break;
                    }
                    iterator.MoveNext();
                }
            }
            else if (REGEX_IDENTIFIER_START.IsMatch(c.ToString()))
            {
                string identifier = c + ScanRegex(iterator, REGEX_IDENTIFIER);
                TokenType type = KEYWORDS.Contains(identifier)
                    ? TokenType.Keyword
                    : TokenType.Identifier;

                yield return new Token<TokenType>(type, identifier, line, col);
            }
            else if (REGEX_NUMBER_DEC.IsMatch(c.ToString()))
            {
                yield return new Token<TokenType>(
                    TokenType.Number,
                    ScanNumber(iterator, c.ToString()),
                    line,
                    col
                );
            }
            else if (WHITESPACE.Contains(c))
            {
                continue;
            }
            else if (c == TYPE_ANNOTATION)
            {
                yield return new Token<TokenType>(
                    TokenType.TypeAnnotation,
                    c.ToString(),
                    line,
                    col
                );
            }
            else if (GROUPING.Contains(c))
            {
                yield return new Token<TokenType>(
                    TokenType.Grouping,
                    c.ToString(),
                    line,
                    col
                );
            }
            else if (PotentialOperators("").Contains(c.ToString()))
            {
                string op = c.ToString();
                c = iterator.MoveNext();

                while (PotentialOperators(op).Contains(c.ToString()))
                {
                    op += c;
                    c = iterator.MoveNext();
                }

                iterator.MoveBack();

                yield return new Token<TokenType>(
                    TokenType.Operator,
                    op,
                    line,
                    col
                );
            }
            else if (DELIMITERS.Contains(c))
            {
                yield return new Token<TokenType>(
                    TokenType.Punctuation,
                    c.ToString(),
                    line,
                    col
                );
            }
            else if (c == END)
            {
                yield return new Token<TokenType>(
                    TokenType.End,
                    c.ToString(),
                    line,
                    col
                );
            }
        }
    }
}
