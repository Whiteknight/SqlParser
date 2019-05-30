using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlInfixOperationNode : SqlNode
    {
        public SqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public SqlNode Right { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Left.ToString(sb, level);
            if (Operator.Operator == "AND" || Operator.Operator == "OR")
            {
                sb.AppendLine();
                sb.AppendIndent(level);
            }
            else
                sb.Append(" ");

            Operator.ToString(sb, level);
            sb.Append(" ");
            Right.ToString(sb, level);
        }
    }
}
