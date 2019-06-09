namespace CastIron.SqlParsing.Ast
{
    public class SqlUnionStatementNode : SqlNode
    {
        public SqlNode First { get; set; }
        public string Operator { get; set; }
        public SqlNode Second { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            First.ToString(sb);
            sb.AppendLineAndIndent();
            sb.AppendLineAndIndent(Operator);
            Second.ToString(sb);
        }
    }
}