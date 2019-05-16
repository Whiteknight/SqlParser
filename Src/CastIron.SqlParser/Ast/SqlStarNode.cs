using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStarNode : SqlNode
    {
        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append("*");
        }
    }
}
