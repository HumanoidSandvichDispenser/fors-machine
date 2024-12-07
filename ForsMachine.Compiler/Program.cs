using ForsMachine.Compiler.Procedures;

namespace ForsMachine.Compiler;

public static class Program
{
    public static void Main(string[] args)
    {
        // args[0] is file path
        //string source = @"
        //    function fibonacci(n: int16): int16 =
        //        if (n == 0) 0
        //        else if (n == 1) 1
        //        else fibonacci(n - 1) + fibonacci(n - 2);

        //    function factorial(n: int16): int16 =
        //        if (n == 0) 1
        //        else n * factorial(n - 1);

        //    function main(): int16 =
        //        0;
        //";

        string filePath = args[0];

        string source;

        try
        {
            source = System.IO.File.ReadAllText(filePath);
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.Fail(e.Message);
            return;
        }

        Frontend.Tokenizer tokenizer = new();

        var tokens = tokenizer.Lex(new Utils.CharIterator(source)).ToList();

        var parser = new Frontend.Parser(new(tokens));
        var root = new StackFrame(null, "root", null);

        var mainInvocation = new ProcedureInvocation(
            new Symbol("main"),
            new List<Expression>()
        );

        try
        {
            var asm = parser.ParseProgram().SelectMany(exp => exp.GenerateAsm(root)).ToList();

            Console.WriteLine("#include \"rules.asm\"");
            Console.WriteLine(String.Join('\n', mainInvocation.GenerateAsm(root)));
            Console.WriteLine("load r0, rax");
            Console.WriteLine("halt");
            Console.WriteLine(String.Join('\n', asm));

            foreach (var instr in StackFrame.DequeueInstructions())
            {
                Console.WriteLine(String.Join('\n', instr));
            }
            //Console.WriteLine(String.Join('\n', expr.GenerateAsm(new(null, "root" , null))));
        }
        catch (Exceptions.AbstractCompilerException e)
        {
            Console.Error.WriteLine("An error occurred while compiling the program.");
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine(e.Trace?.DumpState());
        }
    }
}

/*

# everything is an expression and returns a value

let my_fun: pointer = operation add(x: int16, y: int16) {
    # operations do not create a new stack frame and the compiler simply
    # replaces the call with the instructions
    # this is akin to inline functions in C
    x + y;
}

# last expression evaluated is implicitly returned
# Note: the compiler won't load to the return register if nothing is capturing
# the returned value
function max(x: int16, y: int16): int16 {
    if (x > y) x;
        else y;
}

function foo(x: int16, y: int16, z: int16): int16 {
    let a: int16 = max(x, y);
}

function fib(n: int16): int16 {
    if (n == 0) 0;
    else if (n == 1) 1;
    else fib(n - 1) + fib(n - 2);
}

function main() {
    let description = match (value) {
        1 = "One"
        2 = "Two"
        _ = {
            let str = "Value is "
            str + value.to_string()
        }
    }

    let description = if (value == 1) {
        "One"
    } else if (value == 2) {
        "Two"
    } else {
        let str = "Value is "
        str + value.to_string()
    }
}

 */
