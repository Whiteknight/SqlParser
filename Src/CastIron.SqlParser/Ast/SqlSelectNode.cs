using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectNode : SqlNode
    {
        public string Modifier { get; set; }
        public SqlSelectTopNode TopClause { get; set; }
        public SqlListNode<SqlNode> Columns { get; set; }
        public SqlSelectFromClauseNode FromClause { get; set; }
        public SqlSelectWhereClauseNode WhereClause { get; set; }
        public SqlSelectOrderByClauseNode OrderByClause { get; set; }
        public SqlSelectGroupByNode GroupByClause { get; set; }

        public SqlSelectHavingClauseNode HavingClause { get; set; }

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
            ToString(sb, TopClause, level);
            Columns.ToString(sb, level);
            ToString(sb, FromClause, level);
            ToString(sb, WhereClause, level);
            ToString(sb, OrderByClause, level);
            ToString(sb, GroupByClause, level);
            ToString(sb, HavingClause, level);
        }

        private void ToString(StringBuilder sb, SqlNode child, int level)
        {
            if (child != null)
            {
                sb.AppendLine();
                sb.AppendIndent(level);
                child.ToString(sb, level);
            }
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

    public class SqlSelectWhereClauseNode : SqlNode
    {
        public SqlNode SearchCondition { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendLine("WHERE");
            sb.AppendIndent(level + 1);
            SearchCondition.ToString(sb, level + 1);
        }
    }

    public class SqlSelectHavingClauseNode : SqlNode
    {
        public SqlNode SearchCondition { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendLine("HAVING");
            sb.AppendIndent(level + 1);
            SearchCondition.ToString(sb, level + 1);
        }
    }
}
