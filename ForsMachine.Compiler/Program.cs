using ForsMachine.Compiler.Procedures;
using ForsMachine.Compiler.Types;

namespace ForsMachine.Compiler;

public static class Program
{
    public static void Main(string[] args)
    {
        //Console.WriteLine(String.Join('\n', AddProcedure.Instance.GenerateAsm(null)));
        //Console.WriteLine(String.Join('\n', ))

        // Write a function G(n) = G(0), G(0) = 0
        var root = new StackFrame("root", null);
        var fibonacci = new Function("fibonacci", new(), TypeSystem.Types["int16"]);
        fibonacci.Parameters.Add("n", TypeSystem.Types["int16"]);
        IEnumerable<Expression> fibonacciInstructions = [
            new Conditional(
                new ProcedureInvocation(
                    ComparisonOperation.LessThanOrEqual,
                    [
                        new Symbol("n"),
                        new Atom(1)
                    ]
                ),
                new Atom(1),
                new ProcedureInvocation(
                    ArithmeticOperation.Add,
                    [
                        new ProcedureInvocation(
                            fibonacci,
                            [
                                new ProcedureInvocation(
                                    ArithmeticOperation.Subtract,
                                    [
                                        new Symbol("n"),
                                        new Atom(1)
                                    ]
                                ),
                            ]
                        ),
                        new ProcedureInvocation(
                            fibonacci,
                            [
                                new ProcedureInvocation(
                                    ArithmeticOperation.Subtract,
                                    [
                                        new Symbol("n"),
                                        new Atom(2)
                                    ]
                                ),
                            ]
                        ),
                    ]
                )
            )
        ];

        fibonacci.Instructions.AddRange(fibonacciInstructions);

        try
        {
            foreach (var line in fibonacci.GenerateAsm(root))
            {
                Console.WriteLine(line);
            }
        }
        catch (Exceptions.AbstractCompilerException e)
        {
            Console.WriteLine(e.Trace?.DumpState());
            throw;
        }
    }
}
