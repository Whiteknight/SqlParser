namespace SqlParser.Ast
{
    public class SqlParenthesisNode<TNode> : SqlNode
        where TNode: SqlNode
    {
        public SqlParenthesisNode()
        {
        }

        public SqlParenthesisNode(TNode subexpression)
        {
            Expression = subexpression;
            Location = subexpression.Location;
        }

        public TNode Expression { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitParenthesis(this);

        public SqlParenthesisNode<TNode> Update(TNode expr)
        {
            if (expr == Expression)
                return this;
            return new SqlParenthesisNode<TNode>
            {
                Location = Location,
                Expression = expr
            };
        }
    }
}