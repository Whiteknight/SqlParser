using System.Text;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStringNode : SqlNode
    {
        public SqlStringNode()
        {
        }

        public SqlStringNode(SqlToken token)
        {
            Value = token.Value;
            Location = token.Location;
        }

        public string Value { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append("'");
            sb.Append(Value.Replace("'", "''"));
            sb.Append("'");
        }
    }
}
