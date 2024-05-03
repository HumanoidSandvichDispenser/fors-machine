namespace ForsMachine.Utils;

public class InterpreterException : System.Exception
{
    public int Line { get; set; }
    public int Column { get; set; }

    public InterpreterException(string msg, int line, int col) : base(msg)
    {
        Line = line;
        Column = col;
    }
}
