namespace ForsMachine.Compiler;

public struct OffsetBinding
{
    private Tuple<Types.Type, int> _binding;

    public OffsetBinding(Types.Type type, int offset)
    {
        _binding = new(type, offset);
    }

    public Types.Type Type
    {
        get => _binding.Item1;
    }

    public int Offset
    {
        get => _binding.Item2;
    }
}
