namespace ForsMachine.Compiler.Procedures;

public class MutationOperation : BinaryOperation
{
    public static MutationOperation Mutate = new("mutate");

    private MutationOperation(string name)
        : base(
            name,
            Types.TypeSystem.Types["unknown"],
            Types.TypeSystem.Types["unknown"],
            Types.TypeSystem.Types["unknown"]
        )
    {

    }

    public override string[] GenerateInvocation(StackFrame stackFrame)
    {
        return [
            ..base.GenerateInvocation(stackFrame),
            "pop r0",
            "pop r1",
            "store r1, &r0",
            "load rax, r1",
        ];
    }

    public override string[]? EvaluateArgs(StackFrame? stackFrame, IEnumerable<Expression>? args)
    {
        if (args?.First() is not DereferenceOperation d)
        {
            if (args?.First() is not Symbol s)
            {

            }
        }

        if (args?.Skip(1).First() is Expression ex)
        {
            return [
                ..ex.GenerateAsm(stackFrame, true),
                "push rax",
            ];
        }

        throw new Exceptions.ArgumentCountMismatchException(
            Name,
            2,
            1,
            stackFrame
        );
    }
}
