using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlInfixOperationNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlNode Operator { get; set; }
        public SqlNode Right { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Left.ToString(sb, level);
            sb.Append(" ");
            Operator.ToString(sb, level);
            sb.Append(" ");
            Right.ToString(sb, level);
        }
    }
}
