namespace SqlParser.Ast
{
    public class SqlValuesNode : SqlNode
    {
        public SqlListNode<SqlListNode<SqlNode>> Values { get; set; }

        public SqlValuesNode Update(SqlListNode<SqlListNode<SqlNode>> values)
        {
            if (values == Values)
                return this;
            return new SqlValuesNode
            {
                Location = Location,
                Values = values
            };
        }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitValues(this);
    }
}