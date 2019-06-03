namespace CastIron.SqlParsing.Ast
{
    public class SqlDeleteNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlWhereNode WhereClause { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("DELETE FROM ");
            Source.ToString(sb);
            sb.Append(" ");
            WhereClause?.ToString(sb);
        }
    }
}