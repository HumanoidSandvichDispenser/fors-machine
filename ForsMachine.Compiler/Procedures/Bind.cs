namespace ForsMachine.Compiler.Procedures;

public class Bind : Procedure
{
    public string BindingName { get; set; }

    public Expression Value { get; set; }

    public Bind(string name, Expression value)
        : base("bind", value.Type, false)
    {
        BindingName = name;
        Value = value;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (stackFrame is null)
        {
            throw new NullReferenceException(nameof(stackFrame));
        }

        int offset = stackFrame.CreateBinding(BindingName, Value.Type);

        string allocation = $"incr rsp, rsp, {Type.Size}  ; allocate {BindingName}";
        string[] store;

        // compiler optimization: if the value is an atom, we can just store it
        // to the binding offset directly

        if (Value is Atom atom && atom.CanStoreImmediate())
        {
            store = [
                $"store {atom.Value}, &rbp, {offset}  ; {BindingName}"
            ];

            if (shouldLoad)
            {
                store = [
                    ..store,
                    $"load rax, {atom.Value}"
                ];
            }
        }
        else
        {
            store = [
                ..Value.GenerateAsm(stackFrame, true),
                $"store rax, &rbp, {offset}  ; {BindingName}"
            ];
        }

        return [
            allocation,
            ..store,
        ];
    }
}
