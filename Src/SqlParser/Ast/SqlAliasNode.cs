using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlAliasNode  : ISqlNode
    {
        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public ISqlNode Source { get; set; }
        public SqlIdentifierNode Alias { get; set; }
        public SqlListNode<SqlIdentifierNode> ColumnNames { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitAlias(this);

        public SqlAliasNode Update(ISqlNode source, SqlIdentifierNode alias, SqlListNode<SqlIdentifierNode> columns)
        {
            if (source == Source && alias == Alias && columns == ColumnNames)
                return this;
            return new SqlAliasNode
            {
                Location = Location,
                Source = source,
                Alias = alias,
                ColumnNames = columns
            };
        }
    }
}