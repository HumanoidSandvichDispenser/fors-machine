using ForsMachine.Utils;

namespace ForsMachine.Compiler.Procedures;

public abstract class Procedure : Expression
{
    public string Name { get; set; }

    public OrderedDictionary<string, Types.Type> Parameters { get; set; }

    public List<Expression> Instructions { get; set; }

    public bool IsUserDefined { get; set; }

    public Procedure(string name, Types.Type returnType, bool isUserDefined = true)
        : this(name, new(), returnType, isUserDefined) { }

    public Procedure(
        string name,
        OrderedDictionary<string, Types.Type> parameters,
        Types.Type returnType,
        bool isUserDefined = true
    ) : base(returnType)
    {
        Name = name;
        Parameters = parameters;
        Instructions = new();
        IsUserDefined = isUserDefined;
    }

    public abstract override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false);

    public virtual string[]? EvaluateArgs(StackFrame? stackFrame, IEnumerable<Expression>? args)
    {
        return args?.Reverse()
            .SelectMany(x => x.GenerateAsm(stackFrame, true).Append("push rax"))
            .ToArray();
    }
}
