using System.Linq;
using System.Text;

namespace ForsMachine.Compiler.Types;

public static class TypeSystem
{
    public static TypeTable Types { get; } = new();

    static TypeSystem()
    {
        Types["int16"] = new Types.Int16();
        Types["pointer"] = new Pointer();
        Types["unknown"] = new UnknownType();
    }

    public static void RegisterStruct(string name, SortedDictionary<string, string> attributes)
    {
        var keypairs = attributes
            .Select((attribute) =>
                new KeyValuePair<string, Type>(
                    attribute.Key, Types[attribute.Value]
                ));

        var typedAttributes = new SortedDictionary<string, Type>();
        foreach (var keypair in keypairs)
        {
            typedAttributes[keypair.Key] = keypair.Value;
        }
        Types[name] = new TypeStruct(name, typedAttributes);
    }

    public static string Dump()
    {
        var builder = new StringBuilder();
        foreach (var type in Types.Dump())
        {
            if (type.Value is TypeStruct typeStruct)
            {
                builder.AppendLine($"{typeStruct.Name}({typeStruct.Size}): {type.Value}");
                foreach (var attribute in typeStruct.Attributes)
                {
                    builder.AppendLine($"\t&{typeStruct.GetOffset(attribute.Key)} {attribute.Key}: {attribute.Value.Name}");
                }
                continue;
            }
            else
            {
                builder.AppendLine($"{type.Key}: {type.Value}");
            }
        }
        return builder.ToString();
    }
}
