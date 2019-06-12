using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlOperatorNode  : SqlNode
    {
        public SqlOperatorNode()
        {
        }

        public SqlOperatorNode(SqlToken token)
        {
            Operator = token.Value;
            Location = token.Location;
        }

        public SqlOperatorNode(string op, Location location)
        {
            Operator = op;
            Location = location;
        }

        public SqlOperatorNode(string op)
        {
            Operator = op;
        }

        public string Operator { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitOperator(this);
    }
}