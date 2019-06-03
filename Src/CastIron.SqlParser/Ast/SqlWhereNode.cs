namespace CastIron.SqlParsing.Ast
{
    public class SqlWhereNode : SqlNode
    {
        public SqlNode SearchCondition { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.AppendLine("WHERE");
            sb.IncreaseIndent();
            sb.WriteIndent();
            SearchCondition.ToString(sb);
            sb.DecreaseIndent();
        }
    }
}