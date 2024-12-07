namespace ForsMachine.Compiler;

public class Scope
{
    public Dictionary<string, OffsetBinding> Bindings { get; set; } = new();

    public Dictionary<string, Expression> CompileTimeBindings { get; set; } = new();

    public Scope? Parent { get; set; }

    public Scope(Scope? parent = null)
    {
        Parent = parent;
    }

    public int CreateBinding(string name, Types.Type type, int offset)
    {
        Bindings[name] = new(type, offset);
        return Bindings[name].Offset;
    }

    public OffsetBinding GetBinding(string name, StackFrame context)
    {
        if (Bindings.ContainsKey(name))
        {
            return Bindings[name];
        }
        else if (Parent is not null)
        {
            return Parent.GetBinding(name, context);
        }
        else
        {
            throw new Exceptions.InvalidBindingException(name, context);
        }
    }

    public Expression CreateCompileTimeBinding(string name, Expression expression)
    {
        CompileTimeBindings[name] = expression;
        return CompileTimeBindings[name];
    }

    public Expression GetCompileTimeBinding(string name, StackFrame caller)
    {
        if (CompileTimeBindings.ContainsKey(name))
        {
            return CompileTimeBindings[name];
        }
        else if (Parent is not null)
        {
            return Parent.GetCompileTimeBinding(name, caller);
        }
        else
        {
            throw new Exceptions.InvalidBindingException(name, caller);
        }
    }
}
