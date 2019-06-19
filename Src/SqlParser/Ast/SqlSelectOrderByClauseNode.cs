namespace SqlParser.Ast
{
    public class SqlSelectOrderByClauseNode : SqlNode
    {
        public SqlListNode<SqlOrderByEntryNode> Entries { get; set; }

        public SqlNode Offset { get; set; }
        public SqlNode Limit { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitOrderBy(this);

        public SqlSelectOrderByClauseNode Update(SqlListNode<SqlOrderByEntryNode> entries, SqlNode offset, SqlNode limit)
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

    public class SqlOrderByEntryNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public string Direction { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitOrderByEntry(this);

        public SqlOrderByEntryNode Update(SqlNode source, string direction)
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