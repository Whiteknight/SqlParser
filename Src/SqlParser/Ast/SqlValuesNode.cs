using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlValuesNode : ISqlNode
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