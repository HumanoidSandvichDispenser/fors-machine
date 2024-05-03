using ForsMachine.Utils;
using ForsMachine.Assembler;

Register p = new(2);
Register q = new(4);

// jump to address stored in q if first bit of p register is 1
// 0x53 0x02 0x04 0x00
var instruction = new JumpInstruction(q, p, false);

System.Console.WriteLine(instruction.ToInstruction().ToString("x8"));

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
