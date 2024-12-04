using System.Linq;
using ForsMachine.Compiler.Exceptions;

namespace ForsMachine.Compiler.Procedures;

public class ProcedureInvocation : Expression
{
    public Procedure Procedure { get; set; }

    public List<Expression> Arguments { get; set; }

    public ProcedureInvocation(Procedure procedure, List<Expression> arguments)
        : base(procedure.Type)
    {
        Procedure = procedure;
        Arguments = arguments;
    }

    private void AssertArgumentCount(StackFrame? stackFrame)
    {
        int paramCount = Procedure.Parameters.Count;
        int argCount = Arguments.Count;
        if (paramCount != argCount)
        {
            throw new ArgumentCountMismatchException(
                Procedure.Name,
                paramCount,
                argCount,
                stackFrame
            );
        }
    }

    private void AssertArgumentTypes(StackFrame? stackFrame)
    {
        foreach (var (parameter, argument) in Procedure.Parameters.Zip(Arguments))
        {
            if (parameter.Value != argument.Type)
            {
                throw new TypeMismatchException(argument.Type, parameter.Value, stackFrame);
            }
        }
    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        var args = Arguments as IEnumerable<Expression>;
        var evaluatedArgs = args?.Reverse()
            .SelectMany(x => x.GenerateAsm(stackFrame, true).Append("push rax"))
            .ToArray();

        // if the procedure is an operation, we can just append its assembly

        string[] asm;

        if (Procedure is Operation op)
        {
            if (stackFrame is null)
            {
                throw new NullReferenceException(nameof(stackFrame));
            }

            asm = op.GenerateInvocation(stackFrame);
        }
        else
        {
            asm = ["push_rpc", "jmp &" + Procedure.Label,];
        }

        AssertArgumentCount(stackFrame);
        AssertArgumentTypes(stackFrame);

        return
        [
            ..evaluatedArgs,
            ..asm,
            // clean up the arguments
            ..args?.Select(_ => "pop").Where(_ => Procedure is not Operation),
        ];
    }
}
