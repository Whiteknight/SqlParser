using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectNode : SqlNode, ISqlSymbolScopeNode
    {
        public string Modifier { get; set; }
        public SqlSelectTopNode TopClause { get; set; }
        public SqlListNode<SqlNode> Columns { get; set; }
        public SqlNode FromClause { get; set; }
        public SqlNode WhereClause { get; set; }
        public SqlSelectOrderByClauseNode OrderByClause { get; set; }
        public SqlNode GroupByClause { get; set; }

        public SqlNode HavingClause { get; set; }
        public SymbolTable Symbols { get; set; }

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
            if (FromClause != null)
            {
                sb.AppendLine("FROM ");
                sb.IncreaseIndent();
                sb.WriteIndent();
                FromClause.ToString(sb);
                sb.DecreaseIndent();
            }

            if (WhereClause != null)
            {
                sb.AppendLineAndIndent();
                sb.AppendLine("WHERE");
                sb.IncreaseIndent();
                sb.WriteIndent();
                WhereClause.ToString(sb);
                sb.DecreaseIndent();
            }

            ToString(sb, OrderByClause);
            if (GroupByClause != null)
            {
                sb.AppendLineAndIndent();
                sb.Append("GROUP BY");
                sb.IncreaseIndent();
                sb.AppendLineAndIndent();
                GroupByClause.ToString(sb);
                sb.DecreaseIndent();
            }
            if (HavingClause != null)
            {
                sb.AppendLineAndIndent();
                sb.AppendLine("HAVING");
                sb.IncreaseIndent();
                sb.WriteIndent();
                HavingClause.ToString(sb);
                sb.DecreaseIndent();
            }

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

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitSelect(this);

        public SqlSelectNode Update(string modifier, SqlSelectTopNode top, SqlListNode<SqlNode> columns, 
            SqlNode from, SqlNode where, SqlSelectOrderByClauseNode orderBy,
            SqlNode groupBy, SqlNode having)
        {
            if (modifier == Modifier && top == TopClause && columns == Columns && from == FromClause &&
                where == WhereClause && orderBy == OrderByClause && groupBy == GroupByClause && having == HavingClause)
                return this;
            return new SqlSelectNode
            {
                Location = Location,
                Columns = columns,
                FromClause = from,
                GroupByClause = groupBy,
                HavingClause = having,
                Modifier = modifier,
                OrderByClause = orderBy,
                TopClause = top,
                WhereClause = where
            };
        }
    }
}
