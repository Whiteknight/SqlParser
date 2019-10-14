using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{ 
    public class SqlDeclareNode : ISqlNode
    {
        public SqlVariableNode Variable { get; set; }

        public ISqlNode DataType { get; set; }
        public ISqlNode Initializer { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitDeclare(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlDeclareNode Update(SqlVariableNode v, ISqlNode dataType, ISqlNode init)
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
