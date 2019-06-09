namespace CastIron.SqlParsing.Ast
{
    public class SqlPrefixOperationNode : SqlNode
    {
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Operator.ToString(sb);
            sb.Append(" ");
            if (Right is SqlInfixOperationNode)
            {
                sb.Append("(");
                Right.ToString(sb);
                sb.Append(")");
            }
            else
                Right.ToString(sb);
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitPrefixOperation(this);

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