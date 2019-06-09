namespace CastIron.SqlParsing.Ast
{
    public class SqlInNode : SqlNode
    {
        public bool Not { get; set; }
        public SqlNode Search { get; set; }
        public SqlListNode<SqlNode> Items { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Search.ToString(sb);
            if (Not)
                sb.Append(" NOT");
            sb.Append(" IN (");
            Items.ToString(sb);
            sb.Append(")");
        }
    }
}
