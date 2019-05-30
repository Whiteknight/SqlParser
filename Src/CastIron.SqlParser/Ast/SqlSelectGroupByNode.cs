using System.Collections.Generic;
using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectGroupByNode : SqlNode
    {
        public SqlListNode<SqlNode> Keys { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.AppendLine("GROUP BY");
            sb.AppendIndent(level + 1);
            Keys.ToString(sb, level + 1);
        }
    }
}