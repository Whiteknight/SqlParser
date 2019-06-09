namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectGroupByNode : SqlNode
    {
        public SqlListNode<SqlNode> Keys { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("GROUP BY");
            sb.IncreaseIndent();
            sb.AppendLineAndIndent();
            Keys.ToString(sb);
            sb.DecreaseIndent();
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitGroupBy(this);

        public SqlSelectGroupByNode Update(SqlListNode<SqlNode> keys)
        {
            if (keys == Keys)
                return this;
            return new SqlSelectGroupByNode
            {
                Location = Location,
                Keys = keys
            };
        }
    }
}