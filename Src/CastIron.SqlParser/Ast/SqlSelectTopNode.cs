using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectTopNode : SqlNode
    {
        public SqlNode Value { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendLine();
            sb.AppendIndent(level);
            sb.Append("TOP (");
            Value.ToString(sb, level);
            sb.Append(")");
            if (Percent)
                sb.Append(" PERCENT");
            if (WithTies)
                sb.Append(" WITH TIES");
        }
    }
}
