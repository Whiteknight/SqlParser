using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlBetweenOperationNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlNode Low { get; set; }
        public SqlNode High { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Left.ToString(sb, level);
            sb.Append(" BETWEEN ");
            Low.ToString(sb, level);
            sb.Append(" AND ");
            High.ToString(sb, level);
        }
    }
}