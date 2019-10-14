using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlJoinNode : ISqlNode
    {
        public ISqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public ISqlNode Right { get; set; }
        public ISqlNode OnCondition { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitJoin(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlJoinNode Update(ISqlNode left, SqlOperatorNode op, ISqlNode right, ISqlNode cond)
        {
            if (left == Left && op == Operator && right == Right && cond == OnCondition)
                return this;
            return new SqlJoinNode
            {
                Location = Location,
                Left = left,
                Operator = op,
                Right = right,
                OnCondition = cond
            };
        }
    }
}
