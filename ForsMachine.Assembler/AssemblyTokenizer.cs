using ForsMachine.Utils;
using System.Text.RegularExpressions;

namespace ForsMachine.Assembler;

public class AssemblyTokenizer : Tokenizer<TokenType>
{
    private HashSet<string> KEYWORDS = new HashSet<string>
    {
        "def",
    };

    protected static readonly Regex REGEX_IDENTIFIER_START = new Regex("[_a-zA-Z]");
    protected static readonly Regex REGEX_IDENTIFIER = new Regex("[_\\-a-zA-Z0-9]");

    public override IEnumerable<Token<TokenType>> Lex(CharIterator iterator)
    {
        int line, col;
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
            else if (c == '.')
            {
                string labelName = ScanRegex(iterator, REGEX_IDENTIFIER);
                bool isDefinition = false;
                if (iterator.GetNext() == ':') {
                    isDefinition = true;
                    iterator.MoveNext();
                }
                yield return new Token<TokenType>(
                    isDefinition ? TokenType.LabelDefinition : TokenType.Label,
                    labelName, line, col);
            }
            else if (REGEX_IDENTIFIER_START.IsMatch(c.ToString()))
            {
                string identifier = c + ScanRegex(iterator, REGEX_IDENTIFIER);
                TokenType type = KEYWORDS.Contains(identifier) ?
                    TokenType.Keyword : TokenType.Identifier;

                yield return new Token<TokenType>(type,
                    identifier, line, col);
            }
            else if (REGEX_NUMBER_DEC.IsMatch(c.ToString()))
            {
                yield return new Token<TokenType>(TokenType.Number,
                    ScanNumber(iterator, c.ToString()), line, col);
            }
            else if (WHITESPACE.Contains(c))
            {
                continue;
            }
            else if (c == '$')
            {
                // register
                yield return new Token<TokenType>(TokenType.Register,
                    "$", line, col);
            }
            else if (c == ',')
            {
                continue;
            }
            else if (c == '\n')
            {
                yield return new Token<TokenType>(TokenType.End, "", line, col);
            }
            else
            {
                throw new InterpreterException($"Unknown symbol '{c}'",
                    line, col);
            }
        }

        line = iterator.Line;
        col = iterator.Column;
        yield return new Token<TokenType>(TokenType.End, "", line, col);
    }
}
