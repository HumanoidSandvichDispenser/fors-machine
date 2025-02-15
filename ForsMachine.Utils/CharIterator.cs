namespace ForsMachine.Utils;

public class CharIterator : Iterator<char>
{
    public int Line { get; protected set; } = 1;

    public int Column { get; protected set; } = 0;

    public CharIterator(string str) : base(str.ToCharArray())
    {

    }

    public CharIterator(char[] chars) : base(chars)
    {

    }

    public override char MoveNext()
    {
        char c = base.MoveNext();
        if (c == '\n')
        {
            Line++;
            Column = 0;
        }
        else
        {
            Column++;
        }
        return c;
    }

    public override void MoveBack()
    {
        if (_elements[Index] == '\n')
        {
            Line--;
        }
        else
        {
            Column--;
        }
        base.MoveBack();
    }
}
