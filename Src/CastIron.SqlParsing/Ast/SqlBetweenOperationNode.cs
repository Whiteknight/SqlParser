namespace CastIron.SqlParsing.Ast
{
    public class SqlBetweenOperationNode : SqlNode
    {
        public bool Not { get; set; }
        public SqlNode Left { get; set; }
        public SqlNode Low { get; set; }
        public SqlNode High { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Left.ToString(sb);
            if (Not)
                sb.Append(" NOT");
            sb.Append(" BETWEEN ");
            Low.ToString(sb);
            sb.Append(" AND ");
            High.ToString(sb);
        }
    }
}