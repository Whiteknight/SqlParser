namespace SqlParser.Ast
{
    public class SqlSetNode : SqlNode
    {
        public SqlVariableNode Variable { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitSet(this);

        public SqlSetNode Update(SqlVariableNode v, SqlOperatorNode op, SqlNode right)
        {
            if (v == Variable && op == Operator && right == Right)
                return this;
            return new SqlSetNode
            {
                Location = Location,
                Right = right,
                Operator = op,
                Variable = v
            };
        }
    }
}