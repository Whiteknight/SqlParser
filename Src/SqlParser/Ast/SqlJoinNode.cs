namespace SqlParser.Ast
{
    public class SqlJoinNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }
        public SqlNode OnCondition { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitJoin(this);

        public SqlJoinNode Update(SqlNode left, SqlOperatorNode op, SqlNode right, SqlNode cond)
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
