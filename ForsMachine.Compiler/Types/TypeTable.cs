namespace ForsMachine.Compiler.Types;

public class TypeTable : Dictionary<string, Type>
{
    public new Type this[string key]
    {
        get
        {
            if (!ContainsKey(key))
            {
                throw new Exceptions.TypeNotFoundException(key);
            }
            return base[key];
        }
        set
        {
            base[key] = value;
        }
    }
}
