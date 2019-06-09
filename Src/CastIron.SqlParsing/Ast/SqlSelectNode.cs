namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectNode : SqlNode
    {
        public string Modifier { get; set; }
        public SqlSelectTopNode TopClause { get; set; }
        public SqlListNode<SqlNode> Columns { get; set; }
        public SqlSelectFromClauseNode FromClause { get; set; }
        public SqlWhereNode WhereClause { get; set; }
        public SqlSelectOrderByClauseNode OrderByClause { get; set; }
        public SqlSelectGroupByNode GroupByClause { get; set; }

        public SqlSelectHavingClauseNode HavingClause { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("SELECT ");
            if (Modifier != null)
            {
                sb.Append(Modifier);
                sb.Append(" ");
            }

            sb.IncreaseIndent();

            ToString(sb, TopClause);
            sb.AppendLineAndIndent();
            Columns.ToString(sb);
            ToString(sb, FromClause);
            ToString(sb, WhereClause);
            ToString(sb, OrderByClause);
            ToString(sb, GroupByClause);
            ToString(sb, HavingClause);

            sb.DecreaseIndent();
        }

        private void ToString(SqlStringifier sb, SqlNode child)
        {
            if (child != null)
            {
                sb.AppendLineAndIndent();
                child.ToString(sb);
            }
        }
    }

    public class SqlSelectHavingClauseNode : SqlNode
    {
        public SqlNode SearchCondition { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.AppendLine("HAVING");
            sb.IncreaseIndent();
            sb.WriteIndent();
            SearchCondition.ToString(sb);
            sb.DecreaseIndent();
        }
    }
}
