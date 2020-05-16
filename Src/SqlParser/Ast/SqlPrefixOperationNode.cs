using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlPrefixOperationNode : SqlNode, ISqlNode
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