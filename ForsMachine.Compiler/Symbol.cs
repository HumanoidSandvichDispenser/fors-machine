namespace ForsMachine.Compiler;

public class Symbol : Expression
{
    public string Name { get; set; }

    public Symbol(string name) : base(Types.TypeSystem.Types["unknown"])
    {
        Name = name;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (stackFrame is null)
        {
            throw new NullReferenceException("Stack frame is null.");
        }

        // type is resolved at compile/code generation time
        var binding = stackFrame.GetBinding(Name);
        Type = binding.Type;

        if (!shouldLoad)
        {
            return [];
        }

        return [
            $"load rax, &rbp, {binding.Offset}  ; {Name}",
        ];
    }
}
