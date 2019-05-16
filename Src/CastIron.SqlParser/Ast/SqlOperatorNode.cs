using System.Text;
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

        public string Operator { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append(Operator);
        }
    }
}