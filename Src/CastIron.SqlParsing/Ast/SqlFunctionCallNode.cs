namespace CastIron.SqlParsing.Ast
{
    public class SqlFunctionCallNode : SqlNode
    {
        public SqlNode Name { get; set; }
        public SqlListNode<SqlNode> Arguments { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Name.ToString(sb);
            sb.Append("(");
            Arguments?.ToString(sb);
            sb.Append(")");
        }
    }
}
