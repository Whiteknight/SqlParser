using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlSetNode : ISqlNode
    {
        public SqlVariableNode Variable { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public ISqlNode Right { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitSet(this);

        

        public Location Location { get; set; }

        public SqlSetNode Update(SqlVariableNode v, SqlOperatorNode op, ISqlNode right)
        {
            if (v == Variable && op == Operator && right == Right)
                return this;
            return new SqlSetNode
            {
                Location = Location,
                Right = right,
                Operator = op,
                Variable = v
            };
        }
    }
}