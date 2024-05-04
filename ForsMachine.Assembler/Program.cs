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
load-q $0x00 $0x02
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

    Dictionary<string, Label> labelDict = new();
    Dictionary<string, Queue<JumpInstruction>> awaitingLabels = new();
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
                nextLabel.NextInstruction = i;
                nextLabel.ResolveAddress(i.Address);
                labelDict.Add(nextLabel.Name, nextLabel);

                // if this label is being waited for, then all jump instructions
                // waiting for this label will be resolved to this label
                if (awaitingLabels.ContainsKey(nextLabel.Name))
                {
                    var queue = awaitingLabels[nextLabel.Name];

                    while (queue.Count > 0)
                    {
                        var jumpInstruction = queue.Dequeue();
                        jumpInstruction.To = nextLabel;
                    }

                    awaitingLabels.Remove(nextLabel.Name);
                }
            }

            if (i is JumpInstruction jump)
            {
                if (jump.To is Label toLabel)
                {
                    if (labelDict.ContainsKey(toLabel.Name))
                    {
                        jump.To = labelDict[toLabel.Name];
                    }
                    else
                    {
                        // if our label is not found, then we wait until we find
                        // a label of the same name
                        if (!awaitingLabels.ContainsKey(toLabel.Name))
                        {
                            awaitingLabels.Add(toLabel.Name, new());
                        }
                        awaitingLabels[toLabel.Name].Enqueue(jump);
                    }
                }
            }

            instructions.Enqueue(i);
        }
    }

    while (instructions.Count > 0)
    {
        var instr = instructions.Dequeue();
        System.Console.WriteLine(instr.ToInstruction().ToString("x8"));
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
