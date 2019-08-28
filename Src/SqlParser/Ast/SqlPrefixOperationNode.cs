namespace SqlParser.Ast
{
    public class SqlPrefixOperationNode : SqlNode
    {
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitPrefixOperation(this);

        public SqlPrefixOperationNode Update(SqlOperatorNode op, SqlNode right)
        {
            if (op == Operator && right == Right)
                return this;
            return new SqlPrefixOperationNode
            {
                Location = Location,
                Operator = op,
                Right = right
            };
        }
    }
}