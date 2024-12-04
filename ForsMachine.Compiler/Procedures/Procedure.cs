namespace ForsMachine.Compiler.Procedures;

public abstract class Procedure : Expression
{
    public string Name { get; set; }

    public string Label => $"{LabelPrefix}proc_{Name}";

    public string LabelPrefix => IsUserDefined ? "" : "extern_";

    public SortedDictionary<string, Types.Type> Parameters { get; set; }

    public List<Expression> Instructions { get; set; }

    public bool IsUserDefined { get; set; }

    public Procedure(string name, Types.Type returnType, bool isUserDefined = true)
        : this(name, new(), returnType, isUserDefined) { }

    public Procedure(
        string name,
        SortedDictionary<string, Types.Type> parameters,
        Types.Type returnType,
        bool isUserDefined = true
    ) : base(returnType)
    {
        Name = name;
        Parameters = parameters;
        Instructions = new();
        IsUserDefined = isUserDefined;
    }

    public StackFrame GenerateStackFrame()
    {
        return new(Name, Parameters);
    }

    public abstract override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false);
}
