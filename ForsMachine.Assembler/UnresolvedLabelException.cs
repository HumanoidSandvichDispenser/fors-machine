namespace ForsMachine.Assembler;

public class UnresolvedLabelException : Exception
{
    public UnresolvedLabelException(string msg) : base(msg)
    {

    }
}
