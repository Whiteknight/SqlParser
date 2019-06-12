using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlNumberNode : SqlNode
    {
        public SqlNumberNode()
        {
        }

        public SqlNumberNode(SqlToken token)
        {
            Value = decimal.Parse(token.Value);
            Location = token.Location;
        }

        public SqlNumberNode(decimal value)
        {
            Value = value;
        }

        public decimal Value { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitNumber(this);
    }
}