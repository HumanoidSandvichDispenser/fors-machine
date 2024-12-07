namespace ForsMachine.Compiler.Procedures;

public class BindSubprocedure : Procedure
{
    public Expression Subprocedure { get; set; }

    public BindingList Bindings { get; set; }

    public BindSubprocedure(BindingList bindings, Expression subprocedure)
        : base("subprocedure_bind", Types.TypeSystem.Types["unknown"], false)
    {
        Bindings = bindings;
        Subprocedure = subprocedure;
    }

    public void ResolveType()
    {
        Type = Subprocedure.Type;
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        if (stackFrame is null)
        {
            throw new NullReferenceException(nameof(stackFrame));
        }

        string[] asm;
        if (Subprocedure is not Prog p)
        {
            Subprocedure = p = new Prog([Subprocedure]);
        }

        foreach (var binding in Bindings.Bindings.Reverse())
        {
            var type = Types.TypeSystem.Types[binding.Value.Key];
            var value = binding.Value.Value;

            if (value is null)
            {
                throw new Exceptions.InvalidBindingException(binding.Key, stackFrame);
            }

            // TODO: make it evaluate not in the scope of the subprocedure but in the
            // scope of the parent procedure to allow for parallel bindings
            var bind = new Bind(binding.Key, binding.Value.Key, value);
            p.Instructions.Insert(0, bind);
        }

        asm = p.GenerateAsm(stackFrame, shouldLoad);

        ResolveType();
        return asm;
    }
}
