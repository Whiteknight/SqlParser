using SqlParser.SqlServer.Stringify;
using SqlParser.SqlServer.Symbols;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlSelectNode : ISqlNode, ISqlSymbolScopeNode
    {
        public string Modifier { get; set; }
        public SqlTopLimitNode TopClause { get; set; }
        public SqlListNode<ISqlNode> Columns { get; set; }
        public ISqlNode FromClause { get; set; }
        public ISqlNode WhereClause { get; set; }
        public SqlSelectOrderByClauseNode OrderByClause { get; set; }
        public ISqlNode GroupByClause { get; set; }

        public ISqlNode HavingClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitSelect(this);

        

        public Location Location { get; set; }

        public SqlSelectNode Update(string modifier, SqlTopLimitNode top, SqlListNode<ISqlNode> columns, 
            ISqlNode from, ISqlNode where, SqlSelectOrderByClauseNode orderBy,
            ISqlNode groupBy, ISqlNode having)
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
