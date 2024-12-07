namespace ForsMachine.Compiler.Procedures;

public class Function : Procedure
{
    private BindingList _untypedParameters;

    //public string Label => $"{LabelPrefix}proc_{name}";

    public bool IsAnonymous { get; private set; } = false;

    public string Label
    {
        get
        {
            return $"{LabelPrefix}proc_{Name}";
        }
    }

    public string LabelPrefix => IsUserDefined ? "" : "extern_";

    private string _returnTypeString;

    public Function(
        string? name,
        BindingList parameters,
        string returnType,
        bool isUserDefined = true
    ) : base(name ?? "lambda", new(), Types.TypeSystem.Types["unknown"], isUserDefined)
    {
        if (name is null)
        {
            IsAnonymous = true;
            Name = $"lambda_{GetHashCode()}";
        }

        _untypedParameters = parameters;
        _returnTypeString = returnType;
    }

    public virtual IEnumerable<string> GenerateBody(StackFrame? stackFrame)
    {
        //foreach (var instruction in Instructions)
        for (int i = 0; i < Instructions.Count; i++)
        {
            var instruction = Instructions[i];

            // if this is the last instruction, we should load the value to rax
            string[] asm;
            if (i == Instructions.Count - 1)
            {
                asm = instruction.GenerateAsm(stackFrame, true);
            }
            else
            {
                asm = instruction.GenerateAsm(stackFrame);
            }

            if (instruction.Type != Type)
            {
                throw new Exceptions.TypeMismatchException(instruction.Type, Type, stackFrame);
            }


            foreach (var line in asm)
            {
                yield return line;
            }
        }
    }

    private void ResolveType(string type)
    {
        Type = Types.TypeSystem.Types[type];
    }

    private void ResolveParameters(StackFrame sf)
    {
        foreach (var parameter in _untypedParameters.Bindings)
        {
            var type = Types.TypeSystem.Types[parameter.Value.Key];
            Parameters.Add(parameter.Key, type);
            sf.CreateParameter(parameter.Key, type);
        }
    }

    public StackFrame GenerateStackFrame(StackFrame? parent)
    {
        var sf = new StackFrame(parent, Name, Parameters);
        sf.AssociatedFunction = this;
        return sf;
    }

    public override string[] GenerateAsm(StackFrame? sf, bool shouldLoad = false)
    {
        var stackFrame = GenerateStackFrame(sf);

        ResolveType(_returnTypeString);
        ResolveParameters(stackFrame);

        // register the function with the stack frame
        sf?.CreateBinding(Name, Type);
        stackFrame.CreateCompileTimeBinding("recur", this);

        if (!IsAnonymous)
        {
            StackFrame.RegisterProcedure(this);
        }

        var parameters = String.Join(", ",
            Parameters.Select(x => $"{x.Key}: {x.Value.Name}"));

        var functionSignature = (IsUserDefined ? "global " : "extern ")
            + $"{Name}({parameters}): {Type.Name}";

        var prologue = stackFrame.Push().Select(x => "\t" + x);
        var epilogue = stackFrame.Pop().Select(x => "\t" + x);
        var body = GenerateBody(stackFrame).Select(x => "\t" + x);

        string[] asm = [
            $"{Label}: ; {functionSignature}",
            ..prologue,
            ..body,
            ..epilogue,
        ];

        if (IsAnonymous)
        {
            StackFrame.EnqueueInstructions(asm);
            if (shouldLoad)
            {
                return [$"load rax, {Label}"];
            }
            return [];
        }
        else
        {
            return asm;
        }
    }
}
