using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlPrefixOperationNode : ISqlNode
    {
        public SqlOperatorNode Operator { get; set; }
        public ISqlNode Right { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitPrefixOperation(this);

        

        public Location Location { get; set; }

        public SqlPrefixOperationNode Update(SqlOperatorNode op, ISqlNode right)
        {
            if (op == Operator && right == Right)
                return this;
            return new SqlPrefixOperationNode
            {
                Location = Location,
                Operator = op,
                Right = right
            };
        }
    }
}