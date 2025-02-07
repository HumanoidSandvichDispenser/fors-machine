using System.Linq;
using ForsMachine.Compiler.Exceptions;

namespace ForsMachine.Compiler.Procedures;

public class ProcedureInvocation : Expression
{
    private Expression _procedureName;

    public Procedure? Procedure { get; set; }

    public List<Expression> Arguments { get; set; }

    public ProcedureInvocation(Expression procedureName, List<Expression> arguments)
        : base(Types.TypeSystem.Types["unknown"])
    {
        _procedureName = procedureName;
        Arguments = arguments;
    }

    private void AssertArgumentCount(StackFrame? stackFrame, Procedure p)
    {
        int paramCount = p.Parameters.Count;
        int argCount = Arguments.Count;
        if (paramCount != argCount)
        {
            throw new ArgumentCountMismatchException(
                p.Name,
                paramCount,
                argCount,
                stackFrame
            );
        }
    }

    private void AssertArgumentTypes(StackFrame? stackFrame, Procedure p)
    {
        foreach (var (parameter, argument) in p.Parameters.Zip(Arguments))
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

        while (_procedureName is OuterExpression expr)
        {
            _procedureName = expr.Subexpression;
        }

        if (_procedureName is Symbol s)
        {
            if (StackFrame.Procedures.ContainsKey(s.Name))
            {
                Procedure = StackFrame.Procedures[s.Name];
            }
            else
            {
                var expr = stackFrame?.GetCompileTimeBinding(s.Name);
                if (expr is not Function f)
                {
                    throw new NotImplementedException();
                }
                Procedure = f;
            }

            Type = Procedure.Type;
        }
        else if (_procedureName is Procedures.Procedure p)
        {
            Procedure = p;

            if (p is Procedures.Function f && f.IsAnonymous)
            {
                f.GenerateAsm(stackFrame);
                //StackFrame.EnqueueInstructions(f.GenerateAsm(stackFrame));
            }

            Type = p.Type;
        }
        else
        {
            throw new NotImplementedException();
        }

        //var evaluatedArgs = args?.Reverse()
        //    .SelectMany(x => x.GenerateAsm(stackFrame, true).Append("push rax"))
        //    .ToArray();
        var evaluatedArgs = Procedure.EvaluateArgs(stackFrame, args);

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
        else if (Procedure is Function f)
        {
            asm = ["push_rpc", "jmp &" + f.Label];
        }
        else
        {
            throw new NotImplementedException();
        }

        AssertArgumentCount(stackFrame, Procedure);
        AssertArgumentTypes(stackFrame, Procedure);

        return
        [
            ..evaluatedArgs,
            ..asm,
            // clean up the arguments
            ..args?.Select(_ => "pop").Where(_ => Procedure is not Operation),
        ];
    }
}
