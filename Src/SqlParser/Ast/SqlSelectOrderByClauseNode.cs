using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlSelectOrderByClauseNode : ISqlNode
    {
        public SqlListNode<SqlOrderByEntryNode> Entries { get; set; }

        public ISqlNode Offset { get; set; }
        public ISqlNode Limit { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitOrderBy(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlSelectOrderByClauseNode Update(SqlListNode<SqlOrderByEntryNode> entries, ISqlNode offset, ISqlNode limit)
        {
            if (entries == Entries && offset == Offset && limit == Limit)
                return this;
            return new SqlSelectOrderByClauseNode
            {
                Location = Location,
                Entries = entries,
                Offset = offset,
                Limit = limit
            };
        }
    }

    public class SqlOrderByEntryNode : ISqlNode
    {
        public ISqlNode Source { get; set; }
        public string Direction { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitOrderByEntry(this);

        public override string ToString() => StringifyVisitor.ToString(this);

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