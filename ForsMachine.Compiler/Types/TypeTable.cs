namespace ForsMachine.Compiler.Types;

public class TypeTable
{
    private Dictionary<string, Type> _types = new();

    private Dictionary<string, PrimitiveType> _primitiveTypes = new();

    public Dictionary<string, Type> Dump()
    {
        return new Dictionary<string, Type>(_types);
    }

    public Type this[string key]
    {
        get
        {
            if (!_types.ContainsKey(key))
            {
                throw new Exceptions.TypeNotFoundException(key);
            }
            return _types[key];
        }
        set
        {
            _types[key] = value;
            if (value is PrimitiveType primitive)
            {
                _primitiveTypes[key] = primitive;
            }
            else if (_primitiveTypes.ContainsKey(key))
            {
                _primitiveTypes.Remove(key);
            }
        }
    }

    public PrimitiveType GetPrimitiveType(string key)
    {
        if (!_primitiveTypes.ContainsKey(key))
        {
            throw new Exceptions.TypeNotFoundException(key);
        }
        return _primitiveTypes[key];
    }
}
