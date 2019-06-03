namespace CastIron.SqlParsing.Ast
{
    public class SqlAliasNode  : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlIdentifierNode Alias { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Source.ToString(sb);
            sb.Append(" AS ");
            Alias.ToString(sb);
        }
    }
}