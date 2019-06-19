namespace SqlParser.Ast
{
    public class SqlAliasNode  : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlIdentifierNode Alias { get; set; }
        public SqlListNode<SqlIdentifierNode> ColumnNames { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitAlias(this);

        public SqlAliasNode Update(SqlNode source, SqlIdentifierNode alias, SqlListNode<SqlIdentifierNode> columns)
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