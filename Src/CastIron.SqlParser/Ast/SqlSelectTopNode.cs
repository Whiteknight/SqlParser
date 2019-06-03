namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectTopNode : SqlNode
    {
        public SqlNode Value { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("TOP (");
            Value.ToString(sb);
            sb.Append(")");
            if (Percent)
                sb.Append(" PERCENT");
            if (WithTies)
                sb.Append(" WITH TIES");
        }
    }
}
