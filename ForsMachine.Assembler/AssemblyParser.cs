using ForsMachine.Utils;

namespace ForsMachine.Assembler;

public class AssemblyParser : Parser<AssemblyExpression, TokenType>
{
    public override AssemblyExpression? NextExpression(AssemblyExpression? prev)
    {
        return null;
    }
}
