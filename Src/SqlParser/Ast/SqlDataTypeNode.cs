namespace SqlParser.Ast
{
    public class SqlDataTypeNode : SqlNode
    {
        public SqlKeywordNode DataType { get; set; }
        public SqlNode Size { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitDataType(this);

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
