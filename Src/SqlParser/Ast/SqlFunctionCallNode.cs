namespace SqlParser.Ast
{
    public class SqlFunctionCallNode : SqlNode
    {
        public SqlNode Name { get; set; }
        public SqlListNode<SqlNode> Arguments { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitFunctionCall(this);

        public SqlFunctionCallNode Update(SqlNode name, SqlListNode<SqlNode> args)
        {
            if (name == Name && args == Arguments)
                return this;
            return new SqlFunctionCallNode
            {
                Location = Location,
                Name = name,
                Arguments = args
            };
        }
    }
}
