namespace CastIron.SqlParsing.Ast
{
    public class SqlDataTypeNode : SqlNode
    {
        public SqlKeywordNode DataType { get; set; }
        public SqlNode Size { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitDataType(this);

        public override void ToString(SqlStringifier sb)
        {
            DataType.ToString(sb);
            if (Size != null)
            {
                sb.Append("(");
                Size.ToString(sb);
                sb.Append(")");
            }
        }

        public SqlDataTypeNode Update(SqlKeywordNode keyword, SqlNode size)
        {
            if (keyword == DataType && size == Size)
                return this;
            return new SqlDataTypeNode
            {
                Location = Location,
                DataType = keyword,
                Size = size
            };
        }
    }
}
