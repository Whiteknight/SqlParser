using System.Text;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlVariableNode : SqlNode
    {
        public SqlVariableNode()
        {
        }

        public SqlVariableNode(SqlToken token)
        {
            Name = token.Value;
            Location = token.Location;
        }

        public string Name { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            if (!Name.StartsWith("@"))
                sb.Append("@");
            sb.Append(Name);
        }
    }
}