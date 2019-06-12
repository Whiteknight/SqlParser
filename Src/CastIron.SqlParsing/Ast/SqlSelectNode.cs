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
