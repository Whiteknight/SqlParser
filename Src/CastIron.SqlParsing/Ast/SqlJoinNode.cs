namespace CastIron.SqlParsing.Ast
{
    public class SqlJoinNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }
        public SqlNode OnCondition { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Left.ToString(sb);
            sb.AppendLineAndIndent();
            Operator.ToString(sb);
            sb.AppendLineAndIndent();
            Right.ToString(sb);
            
            if (OnCondition != null)
            {
                sb.IncreaseIndent();
                sb.AppendLineAndIndent();
                sb.Append("ON ");
                OnCondition.ToString(sb);
                sb.DecreaseIndent();
            }
        }

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
