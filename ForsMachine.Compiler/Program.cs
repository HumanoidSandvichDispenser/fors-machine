namespace ForsMachine.Compiler;

public static class Program
{
    public static void Main(string[] args)
    {
        var attributes = new SortedDictionary<string, string>
        {
            { "prev", "pointer" },
            { "size", "int16" },
            { "next", "pointer" },
        };
        Types.TypeSystem.RegisterStruct("free_list", attributes);
        Console.WriteLine(Types.TypeSystem.Dump());

        StackFrame stackFrame = new();
        Console.WriteLine(String.Join('\n', stackFrame.Push()));
        Console.WriteLine(String.Join('\n', stackFrame.Pop()));
    }
}
