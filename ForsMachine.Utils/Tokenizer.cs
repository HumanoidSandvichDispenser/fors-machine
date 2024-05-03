namespace ForsMachine.Utils;

using System.Text.RegularExpressions;

public abstract class Tokenizer<TokenType> where TokenType : System.Enum
{
    protected static readonly Regex REGEX_NUMBER_HEX = new Regex("[.0-9a-fA-F]");
    protected static readonly Regex REGEX_NUMBER_BIN = new Regex("[.01]");
    protected static readonly Regex REGEX_NUMBER_DEC = new Regex("[.0-9]");
    protected static readonly Regex REGEX_IDENTIFIER_START = new Regex("[_a-zA-Z]");
    protected static readonly Regex REGEX_IDENTIFIER = new Regex("[_a-zA-Z0-9]");
    protected static readonly HashSet<char> NUMBER_BASES = new HashSet<char>
    {
        'b',
        'd',
        'x'
    };

    public readonly HashSet<char> WHITESPACE = new HashSet<char>
    {
        ' ',
        '\t',
    };

    public abstract IEnumerable<Token<TokenType>> Lex(CharIterator iterator);

    protected static string ScanString(CharIterator iterator, string begin = "")
    {
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();
            if (c != '"')
            {
                if (c == '\\')
                {
                    char escape = iterator.MoveNext();
                    if (escape == '\0')
                    {
                        throw new InterpreterException("Unexpected EOL",
                            iterator.Line, iterator.Column);
                    }
                    else if (escape == 'n')
                    {
                        begin += '\n';
                    }
                }
                else
                {
                    begin += c;
                }
            }
            else
            {
                return begin;
            }
        }
        throw new InterpreterException("Unexpected EOL",
            iterator.Line, iterator.Column);
    }

    public static string ScanNumber(CharIterator iterator, string begin = "")
    {
        bool hasPoint = false;
        Regex numberBase = REGEX_NUMBER_DEC;

        if (begin == ".")
        {
            hasPoint = true;
        }

        if (begin == "0")
        {
            switch (iterator.GetNext())
            {
                case 'b':
                    numberBase = REGEX_NUMBER_BIN;
                    iterator.MoveNext();
                    break;
                case 'd':
                    numberBase = REGEX_NUMBER_DEC;
                    iterator.MoveNext();
                    break;
                case 'x':
                    numberBase = REGEX_NUMBER_HEX;
                    iterator.MoveNext();
                    break;
            }
        }

        while (iterator.GetNext() != '\0')
        {
            char c = iterator.GetNext();
            if (c == '.')
            {
                if (hasPoint)
                {
                    throw new InterpreterException("Unexpected decimal point",
                        iterator.Line, iterator.Column);
                }
                hasPoint = true;
                begin += c;
            }
            else if (c == ',')
            {
                continue;
            }
            else if (numberBase.IsMatch(c.ToString()))
            {
                begin += c;
            }
            else
            {
                break;
            }
            iterator.MoveNext();
        }
        return begin;
    }

    protected static string ScanRegex(CharIterator iterator, Regex regex)
    {
        string ret = "";
        while (iterator.GetNext() != '\0')
        {
            char c = iterator.MoveNext();

            if (!regex.IsMatch(c.ToString()))
            {
                iterator.MoveBack();
                return ret;
            }

            ret += c;
        }
        return ret;
    }
}
