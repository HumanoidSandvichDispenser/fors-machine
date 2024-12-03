namespace ForsMachine.Compiler;

public struct StackFrame
{
    // format:
    // &0: <return address>
    // &1: <old base pointer>
    // &2+: <local variables>

    public string[] Push()
    {
        return [
            "push rpc",
            "push rbp",
            "load rbp, rsp",
        ];
    }

    public string[] LoadRbpOffsetValueToRegister(string register, string temp, int offset)
    {
        var getAddress = LoadRbpOffsetToRegister(register, temp, offset);
        return [
            ..getAddress,
            $"load {register}, &{register}",
        ];
    }

    public string[] LoadRbpOffsetToRegister(string register, string temp, int offset)
    {
        // load register with rbp
        // add offset to register
        string load = $"load {register}, rbp";
        string loadImmediate = $"load {temp}, {offset}";
        string addOffset = $"add {register}, {register}, {temp}";
        string loadNewRbp = $"load {register}, &{register}";
        return [ load, loadImmediate, addOffset, loadNewRbp ];
    }

    public string LoadReturnAddress(string register)
    {
        return $"load {register}, rbp";
    }

    public string[] Pop()
    {
        var oldRbpToR0 = LoadRbpOffsetToRegister("r0", "r1", 1);
        return [
            "leave",
            ..oldRbpToR0,
            "load rbp, r0",
        ];
    }
}
