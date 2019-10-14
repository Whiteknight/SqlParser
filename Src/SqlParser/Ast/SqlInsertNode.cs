using SqlParser.SqlServer.Stringify;
using SqlParser.SqlServer.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlInsertNode : ISqlNode, ISqlSymbolScopeNode
    {
        public ISqlNode Table { get; set; }
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public ISqlNode Source { get; set; }
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitInsert(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlInsertNode Update(ISqlNode table, SqlListNode<SqlIdentifierNode> columns, ISqlNode source)
        {
            if (table == Table && columns == Columns && source == Source)
                return this;
            return new SqlInsertNode
            {
                Location = Location,
                Table = table,
                Columns = columns,
                Source = source
            };
        }
    }
}