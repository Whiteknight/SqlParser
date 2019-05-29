using System.Collections.Generic;
using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectNode : SqlNode
    {
        public SqlSelectNode()
        {
            Columns = new List<SqlNode>();
        }

        public string Modifier { get; set; }
        public SqlSelectTopNode Top { get; set; }
        public List<SqlNode> Columns { get; }
        public SqlSelectFromClauseNode FromClause { get; set; }
        public SqlSelectOrderByClauseNode OrderBy { get; set; }
        public SqlNode GroupBy { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.Append("SELECT ");
            if (Modifier != null)
            {
                sb.Append(Modifier);
                sb.Append(" ");
            }
            level++;
            Top?.ToString(sb, level);
            foreach (var column in Columns)
            {
                sb.AppendLine();
                sb.AppendIndent(level);
                column.ToString(sb, level);
            }

            sb.AppendLine();
            FromClause?.ToString(sb, level);
            OrderBy?.ToString(sb, level);
            GroupBy?.ToString(sb, level);
        }
    }

    public class SqlSelectSubexpressionNode : SqlNode
    {
        public SqlNode Select { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendLine("(");
            Select?.ToString(sb, level + 1);
            sb.AppendLine();
            sb.AppendIndent(level);
            sb.Append(")");
        }
    }
}
