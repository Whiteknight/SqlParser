using System.Text;
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

        public decimal Value { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append(Value);
        }
    }
}