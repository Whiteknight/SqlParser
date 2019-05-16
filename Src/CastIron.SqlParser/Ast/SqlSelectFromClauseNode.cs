using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectFromClauseNode : SqlNode
    {
        public SqlNode Source { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.AppendLine("FROM ");
            sb.AppendIndent(level + 1);
            Source.ToString(sb, level + 1);
        }
    }
}