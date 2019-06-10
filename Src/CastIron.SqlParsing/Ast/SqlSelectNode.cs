using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectNode : SqlNode, ISqlSymbolScopeNode
    {
        public string Modifier { get; set; }
        public SqlSelectTopNode TopClause { get; set; }
        public SqlListNode<SqlNode> Columns { get; set; }
        public SqlSelectFromClauseNode FromClause { get; set; }
        public SqlWhereNode WhereClause { get; set; }
        public SqlSelectOrderByClauseNode OrderByClause { get; set; }
        public SqlSelectGroupByNode GroupByClause { get; set; }

        public SqlSelectHavingClauseNode HavingClause { get; set; }
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

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitSelect(this);

        public SqlSelectNode Update(string modifier, SqlSelectTopNode top, SqlListNode<SqlNode> columns, 
            SqlSelectFromClauseNode from, SqlWhereNode where, SqlSelectOrderByClauseNode orderBy, 
            SqlSelectGroupByNode groupBy, SqlSelectHavingClauseNode having)
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

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitHaving(this);

        public SqlSelectHavingClauseNode Update(SqlNode search)
        {
            if (search == SearchCondition)
                return this;
            return new SqlSelectHavingClauseNode
            {
                Location = Location,
                SearchCondition = search
            };
        }
    }
}
