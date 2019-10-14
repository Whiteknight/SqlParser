using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlDataTypeNode : ISqlNode
    {
        public SqlKeywordNode DataType { get; set; }
        public ISqlNode Size { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitDataType(this);

        

        public Location Location { get; set; }

        public SqlDataTypeNode Update(SqlKeywordNode keyword, ISqlNode size)
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
