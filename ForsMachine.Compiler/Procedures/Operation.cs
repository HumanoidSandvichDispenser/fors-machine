// alias TypeKV to KeyValuePair<string, Types.Type>

namespace ForsMachine.Compiler.Procedures;

/// <summary>
/// Procedures that do not create a stack frame.
/// </summary>
public abstract class Operation : Procedure
{
    public Operation(string name, Types.Type returnType, bool isUserDefined = false)
        : base(name, returnType, isUserDefined)
    {

    }

    public override string[] GenerateAsm(StackFrame? stackFrame, bool shouldLoad = false)
    {
        // an operation definition does not need to generate any assembly,
        // because it is essentially just a placeholder for a series of
        // instructions

        // the actual assembly generation is done by the calling procedure
        // which calls GenerateInvocation
        return [];
    }

    /// <summary>
    /// Generate the invocation of the operation.
    /// </summary>
    public virtual string[] GenerateInvocation(StackFrame stackFrame)
    {
        return [
            $"; op {Name}",
        ];
    }
}
