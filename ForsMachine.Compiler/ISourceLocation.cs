namespace ForsMachine.Compiler;

public interface ISourceLocation
{
    public int? Line { get; set; }
    public int? Column { get; set; }
}
