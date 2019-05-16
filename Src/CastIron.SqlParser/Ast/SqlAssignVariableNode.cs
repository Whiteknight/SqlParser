using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlAssignVariableNode : SqlNode
    {
        public SqlVariableNode Variable { get; set; }
        public SqlNode RValue { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Variable.ToString(sb, level);
            sb.Append(" = ");
            RValue.ToString(sb, level);
        }
    }
}