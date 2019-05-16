using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlJoinNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }
        public SqlNode OnCondition { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Left.ToString(sb, level);
            sb.AppendLine();
            sb.AppendIndent(level);
            Operator.ToString(sb, level);
            sb.AppendLine();
            sb.AppendIndent(level);
            Right.ToString(sb, level);
            
            if (OnCondition != null)
            {
                sb.AppendLine();
                sb.AppendIndent(level + 1);
                sb.Append("ON ");
                OnCondition.ToString(sb, level + 1);
            }
            
        }
    }
}
