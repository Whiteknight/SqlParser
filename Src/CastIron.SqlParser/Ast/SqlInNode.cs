using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlInNode : SqlNode
    {
        public SqlNode Search { get; set; }
        public SqlListNode<SqlNode> Items { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append(" IN (");
            Items.ToString(sb, level);
            sb.Append(")");
        }
    }
}
