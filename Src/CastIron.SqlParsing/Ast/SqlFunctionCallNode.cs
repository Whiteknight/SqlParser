namespace CastIron.SqlParsing.Ast
{
    public class SqlFunctionCallNode : SqlNode
    {
        public SqlIdentifierNode Name { get; set; }
        public SqlListNode<SqlNode> Arguments { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitFunctionCall(this);

        public SqlFunctionCallNode Update(SqlIdentifierNode name, SqlListNode<SqlNode> args)
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
