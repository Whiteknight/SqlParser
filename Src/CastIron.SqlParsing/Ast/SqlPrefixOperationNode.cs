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
    }
}