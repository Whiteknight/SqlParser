using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{ 
    public class SqlDeclareNode : SqlNode, ISqlNode
    {
        public ISqlNode Variable { get; set; }

        public ISqlNode DataType { get; set; }
        public ISqlNode Initializer { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitDeclare(this);

        public Location Location { get; set; }

        public SqlDeclareNode Update(ISqlNode v, ISqlNode dataType, ISqlNode init)
        {
            if (v == Variable && dataType == DataType && init == Initializer)
                return this;
            return new SqlDeclareNode
            {
                Location = Location,
                DataType = dataType,
                Initializer = init,
                Variable = v
            };
        }
    }
}
