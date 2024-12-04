namespace ForsMachine.Compiler;

public class StackFrame
{
    /// <summary>
    /// The bindings of each name to a memory address offset.
    /// </summary>
    public Dictionary<string, Stack<OffsetBinding>> Bindings { get; set; } = new();

    internal string Identifier { get; set; }

    private int _size = 1;

    public StackFrame(string identifier, SortedDictionary<string, Types.Type>? parameters)
    {
        Identifier = identifier;
        int offset = -2;
        if (parameters is not null)
        {
            foreach (var (name, type) in parameters)
            {
                Bindings[name] = new();
                Bindings[name].Push(new(type, offset));
                offset -= type.Size;
            }
        }
    }

    /// <summary>
    /// Gets the offset of the active binding for the given name.
    /// </summary>
    public int GetBindingOffset(string name)
    {
        return GetBinding(name).Offset;
    }

    public OffsetBinding GetBinding(string name)
    {
        if (!Bindings.ContainsKey(name))
        {
            throw new Exceptions.InvalidBindingException(name);
        }
        return Bindings[name].Peek();
    }

    /// <summary>
    /// Creates a new binding for the given name and type.
    /// </summary>
    public int CreateBinding(string name, Types.Type type)
    {
        if (!Bindings.ContainsKey(name))
        {
            Bindings[name] = new();
        }

        Bindings[name].Push(new(type, _size));
        _size += type.Size;

        return GetBindingOffset(name);
    }

    // format, relative to rbp:
    // &-2+: <arguments>
    // &-1: <return address>
    // &0: <old base pointer>
    // &1+: <local variables>

    public string[] Push()
    {
        return [
            "; prologue",
            "push rbp",
            "load rbp, rsp",
        ];
    }

    public string LoadReturnAddress(string register)
    {
        return $"load {register}, rbp";
    }

    public string[] Pop()
    {
        // current rbp points to old rbp
        return [
            //"leave",
            //"load r0, rbp",
            //"load r1, 1",
            //"sub r0, r0, r1",
            //"load rbp, &rbp",
            //"jmp r0",
            "; epilogue",
            "leave",
            "pop rbp",
            "pop r0",
            "jmp r0"
        ];
    }

    /// <summary>
    /// Dumps the current state of the stack frame.
    /// </summary>
    public string DumpState()
    {
        var title = $"Stack Frame({_size}) - {Identifier}";
        var bindings = String.Join('\n', Bindings.Select(kvp => $"{kvp.Key}: {kvp.Value.Peek()}"));
        return $"{title}\n{bindings}";
    }
}
