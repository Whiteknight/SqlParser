using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlInsertNode : ISqlNode, ISqlSymbolScopeNode
    {
        public ISqlNode Table { get; set; }
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public ISqlNode Source { get; set; }
        public ISqlNode OnConflict { get; set; }
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitInsert(this);

        public Location Location { get; set; }

        public SqlInsertNode Update(ISqlNode table, SqlListNode<SqlIdentifierNode> columns, ISqlNode source, ISqlNode onConflict)
        {
            if (table == Table && columns == Columns && source == Source && onConflict == OnConflict)
                return this;
            return new SqlInsertNode
            {
                Location = Location,
                Table = table,
                Columns = columns,
                Source = source,
                OnConflict = onConflict
            };
        }
    }
}