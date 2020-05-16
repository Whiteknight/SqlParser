using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlValuesNode : SqlNode, ISqlNode
    {
        public SqlListNode<SqlListNode<ISqlNode>> Values { get; set; }

        public SqlValuesNode Update(SqlListNode<SqlListNode<ISqlNode>> values)
        {
            if (values == Values)
                return this;
            return new SqlValuesNode
            {
                Location = Location,
                Values = values
            };
        }

        public Location Location { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitValues(this);
    }
}