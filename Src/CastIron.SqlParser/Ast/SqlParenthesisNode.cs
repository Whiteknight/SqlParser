namespace CastIron.SqlParsing.Ast
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

        public override void ToString(SqlStringifier sb)
        {
            sb.AppendLine("(");
            sb.IncreaseIndent();
            sb.WriteIndent();
            Expression?.ToString(sb);
            sb.DecreaseIndent();
            sb.AppendLineAndIndent();
            sb.Append(")");
        }
    }
}