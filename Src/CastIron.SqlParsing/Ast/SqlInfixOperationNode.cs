namespace CastIron.SqlParsing.Ast
{
    public class SqlInfixOperationNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            ToStringChild(Left, sb);
            if (Operator.Operator == "AND" || Operator.Operator == "OR")
                sb.AppendLineAndIndent();
            else
                sb.Append(" ");

            Operator.ToString(sb);
            sb.Append(" ");
            ToStringChild(Right, sb);
        }

        private void ToStringChild(SqlNode node, SqlStringifier sb)
        {
            if (node is SqlInfixOperationNode)
            {
                sb.Append("(");
                node.ToString(sb);
                sb.Append(")");
                return;
            }

            node.ToString(sb);
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitInfixOperation(this);

        public SqlInfixOperationNode Update(SqlNode left, SqlOperatorNode op, SqlNode right)
        {
            if (left == Left && op == Operator && right == Right)
                return this;
            return new SqlInfixOperationNode
            {
                Location = Location,
                Left = left,
                Operator = op,
                Right = right
            };
        }
    }
}
