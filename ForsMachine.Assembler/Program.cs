using ForsMachine.Utils;
using ForsMachine.Assembler;
using ForsMachine.Assembler.Expressions;
using ForsMachine.Assembler.Instructions;

//Register p = new(2);
//Register q = new(4);

// jump to address stored in q if first bit of p register is 1
// 0x53 0x02 0x04 0x00
//var instruction = new JumpInstruction(q, p, false);

//System.Console.WriteLine(instruction.ToInstruction().ToString("x8"));

string input = @"
load   $0x02 4096
labelname:
load-q $0x00 $0x02
jump labelname
";

try
{
    CharIterator iterator = new(input);
    AssemblyTokenizer tokenizer = new();
    var tokens = tokenizer.Lex(iterator).ToList();

    foreach (var token in tokens)
    {
        System.Console.WriteLine(token.ToString());
    }

    AssemblyParser parser = new(new(tokens));
    Stack<Label> labels = new();

    Dictionary<string, uint> symbolTable = new();
    //Dictionary<string, Queue<JumpInstruction>> awaitingLabels = new();
    Queue<Instruction> instructions = new();

    ushort address = 0;

    foreach (AssemblyExpression expr in parser.Parse())
    {
        if (expr is Label label)
        {
            labels.Push(label);
        }

        if (expr is Instruction i)
        {
            i.Address = address++;

            while (labels.Count > 0)
            {
                // pop all labels to point instruction to here
                var nextLabel = labels.Pop();
                symbolTable.Add(nextLabel.Name, i.Address);
            }

            instructions.Enqueue(i);
        }
    }

    while (instructions.Count > 0)
    {
        var instr = instructions.Dequeue();
        System.Console.WriteLine(instr.ToInstruction(symbolTable).ToString("x8"));
    }
}
catch (InterpreterException ex)
{
    Console.Error.WriteLine("Assembler error:");
    Console.Error.WriteLine($"{ex.Message} Line {ex.Line}, col {ex.Column}");
    string[] lines = input.Split("\n");
    Console.Error.WriteLine(lines[ex.Line - 1]);
    string marker = new string(' ', ex.Column - 1) + '^';
    Console.Error.WriteLine(marker);
}

//Token<TokenType>[] tokens = new Token<TokenType>[]
//{
//    new(TokenType.Identifier, "load", 0, 0),
//    new(TokenType.Register, "$", 0, 0),
//    new(TokenType.Number, "0x00", 0, 0),
//    new(TokenType.Number, "0x00", 0, 0),
//};

//string input = "";
//string? buf;
//
//while ((buf = Console.ReadLine()) is not null)
//{
//    input += buf + '\n';
//}
//
//CharIterator iterator = new(input);
//ForsMachine.Assembler.AssemblyTokenizer tokenizer = new();
//var list = tokenizer.Lex(iterator).ToList();
//
//foreach (var token in list)
//{
//    System.Console.WriteLine(token);
//}

return 0;
