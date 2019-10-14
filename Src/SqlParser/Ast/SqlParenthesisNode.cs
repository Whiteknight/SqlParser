using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlParenthesisNode<TNode> : ISqlNode
        where TNode: class, ISqlNode
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

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitParenthesis(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

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