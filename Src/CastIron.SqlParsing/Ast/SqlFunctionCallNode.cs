namespace CastIron.SqlParsing.Ast
{
    public class SqlFunctionCallNode : SqlNode
    {
        public SqlIdentifierNode Name { get; set; }
        public SqlListNode<SqlNode> Arguments { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Name.ToString(sb);
            sb.Append("(");
            Arguments?.ToString(sb);
            sb.Append(")");
        }

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
