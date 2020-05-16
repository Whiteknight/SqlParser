using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlOrderByNode : SqlNode, ISqlNode
    {
        public SqlListNode<SqlOrderByEntryNode> Entries { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitOrderBy(this);

        public Location Location { get; set; }

        public SqlOrderByNode Update(SqlListNode<SqlOrderByEntryNode> entries)
        {
            if (entries == Entries)
                return this;
            return new SqlOrderByNode
            {
                Location = Location,
                Entries = entries
            };
        }
    }

    public class SqlOrderByEntryNode : SqlNode, ISqlNode
    {
        public ISqlNode Source { get; set; }
        public string Direction { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitOrderByEntry(this);

        public Location Location { get; set; }

        public SqlOrderByEntryNode Update(ISqlNode source, string direction)
        {
            if (source == Source && direction == Direction)
                return this;
            return new SqlOrderByEntryNode
            {
                Location = Location,
                Source = source,
                Direction = direction
            };
        }
    }
}