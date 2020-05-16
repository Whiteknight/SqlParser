using ParserObjects;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlSelectNode : SqlNode, ISqlNode, ISqlSymbolScopeNode
    {
        public string Modifier { get; set; }
        public SqlTopLimitNode TopLimitClause { get; set; }
        public SqlListNode<ISqlNode> Columns { get; set; }
        public ISqlNode FromClause { get; set; }
        public ISqlNode WhereClause { get; set; }
        public SqlOrderByNode OrderByClause { get; set; }
        public ISqlNode GroupByClause { get; set; }
        public ISqlNode HavingClause { get; set; }
        public ISqlNode OffsetClause { get; set; }
        public ISqlNode FetchClause { get; set; }
        
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitSelect(this);

        public Location Location { get; set; }

        public SqlSelectNode Update(string modifier, SqlTopLimitNode top, SqlListNode<ISqlNode> columns, 
            ISqlNode from, ISqlNode where, SqlOrderByNode orderBy,
            ISqlNode groupBy, ISqlNode having, ISqlNode offset, ISqlNode fetch)
        {
            if (modifier == Modifier && top == TopLimitClause && columns == Columns && from == FromClause &&
                where == WhereClause && orderBy == OrderByClause && groupBy == GroupByClause && having == HavingClause &&
                fetch == FetchClause && offset == OffsetClause)
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
                TopLimitClause = top,
                WhereClause = where,
                FetchClause = fetch,
                OffsetClause = offset
            };
        }
    }
}
