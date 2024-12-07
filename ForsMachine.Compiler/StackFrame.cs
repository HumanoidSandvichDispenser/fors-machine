using ForsMachine.Utils;

namespace ForsMachine.Compiler;

public class StackFrame
{
    /// <summary>
    /// The bindings of each name to a memory address offset.
    /// </summary>
    //public Dictionary<string, Stack<OffsetBinding>> Bindings { get; set; } = new();

    public StackFrame? Parent { get; set; }

    private Scope _activeScope = new();

    public static Queue<string[]> AsmQueue { get; set; } = new();

    public static Dictionary<string, Procedures.Procedure> Procedures { get; set; } = new();

    public static void RegisterProcedure(Procedures.Procedure procedure)
    {
        Procedures[procedure.Name] = procedure;
    }

    public Expression? GetCompileTimeBinding(string name)
    {
        return _activeScope.GetCompileTimeBinding(name, this);
    }

    public Expression CreateCompileTimeBinding(string name, Expression expression)
    {
        return _activeScope.CreateCompileTimeBinding(name, expression);
    }

    public Procedures.Function? AssociatedFunction { get; set; }

    internal string Identifier { get; set; }

    private int _size = 1;

    private int _parameterBottom = -1;

    public StackFrame(StackFrame? parent, string identifier, OrderedDictionary<string, Types.Type>? parameters)
    {
        Identifier = identifier;
        int offset = -2;
        if (parameters is not null)
        {
            foreach (var (name, type) in parameters)
            {
                _activeScope.CreateBinding(name, type, offset);
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
        return _activeScope.GetBinding(name, this);
    }

    /// <summary>
    /// Creates a new binding for the given name and type.
    /// </summary>
    public int CreateBinding(string name, Types.Type type)
    {
        var binding = _activeScope.CreateBinding(name, type, _size);
        _size += type.Size;
        return binding;
    }

    public int CreateParameter(string name, Types.Type type)
    {
        _parameterBottom -= type.Size;
        return _activeScope.CreateBinding(name, type, _parameterBottom);
    }

    public void EnterScope()
    {
        _activeScope = new(_activeScope);
    }

    public void ExitScope()
    {
        if (_activeScope.Parent is null)
        {
            // this is usually not the fault of the user/programmer
            throw new InvalidOperationException();
        }

        _activeScope = _activeScope.Parent;
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
        string bindings = "";
        for (var scope = _activeScope; scope is not null; scope = scope.Parent)
        {
            bindings += "---\n";
            bindings += String.Join('\n', scope.Bindings.Select(kvp => $"{kvp.Key}: {kvp.Value.Offset}"));
        }

        var parent = Parent?.DumpState() ?? "";
        return $"{title}\n{bindings}" + (Parent is not null ? $"\n===\n{parent}" : "");
    }

    public static void EnqueueInstructions(string[] asm)
    {
        AsmQueue.Enqueue(asm);
    }

    public static IEnumerable<string[]> DequeueInstructions()
    {
        while (AsmQueue.TryDequeue(out string[]? asm))
        {
            yield return asm;
        }
    }
}
