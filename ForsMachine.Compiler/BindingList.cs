using ForsMachine.Utils;

namespace ForsMachine.Compiler;

public class BindingList : ISourceLocation
{
    public int? Line { get; set; }

    public int? Column { get; set; }

    public OrderedDictionary<string, KeyValuePair<string, Expression?>> Bindings { get; set; } =
        new();
}
