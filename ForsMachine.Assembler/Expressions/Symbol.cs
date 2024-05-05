using ForsMachine.Utils;

namespace ForsMachine.Assembler.Expressions;

public class Symbol : Value
{
    public string Name { get; set; }

    public Symbol(Token<TokenType> source, string name) : base(source)
    {
        Name = name;
    }

    public override uint Evaluate(Dictionary<string, uint> symbolTable)
    {
        if (symbolTable.ContainsKey(Name))
        {
            return symbolTable[Name];
        }

        throw new InterpreterException($"Symbol \"{Name}\" is unassigned.",
            Source.Line, Source.Column);
    }
}
