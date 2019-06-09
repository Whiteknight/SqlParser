namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectFromClauseNode : SqlNode
    {
        public SqlNode Source { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.AppendLine("FROM ");
            sb.IncreaseIndent();
            sb.WriteIndent();
            Source.ToString(sb);
            sb.DecreaseIndent();
        }
    }
}