namespace ForsMachine.Compiler.Procedures;

public class Function : Procedure
{
    public Function(
        string name,
        SortedDictionary<string, Types.Type> parameters,
        Types.Type returnType,
        bool isUserDefined = true
    ) : base(name, parameters, returnType, isUserDefined) { }

    public virtual IEnumerable<string> GenerateBody(StackFrame? stackFrame)
    {
        //foreach (var instruction in Instructions)
        for (int i = 0; i < Instructions.Count; i++)
        {
            var instruction = Instructions[i];
            if (instruction.Type != Type)
            {
                throw new Exceptions.TypeMismatchException(instruction.Type, Type);
            }

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

            foreach (var line in asm)
            {
                yield return line;
            }
        }
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        stackFrame = GenerateStackFrame();

        var parameters = String.Join(", ",
            Parameters.Select(x => $"{x.Key}: {x.Value.Name}"));

        var functionSignature = (IsUserDefined ? "global " : "extern ")
            + $"{Name}({parameters}): {Type.Name}";
            //? $"global {Label}({ parameters }): {Type.Name}"
            //: $"extern {Label}({ parameters }): {Type.Name}";

        var prologue = stackFrame.Push().Select(x => "\t" + x);
        var epilogue = stackFrame.Pop().Select(x => "\t" + x);
        var body = GenerateBody(stackFrame).Select(x => "\t" + x);

        return [
            $"{Label}: ; {functionSignature}",
            ..prologue,
            ..body,
            ..epilogue,
        ];
    }
}
