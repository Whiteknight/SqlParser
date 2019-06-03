using System.Collections.Generic;

namespace CastIron.SqlParsing.Ast
{
    public class SqlCaseNode : SqlNode
    {
        public SqlCaseNode()
        {
            WhenExpressions = new List<SqlCaseWhenNode>();
        }

        public SqlNode InputExpression { get; set; }
        public List<SqlCaseWhenNode> WhenExpressions { get; set; }
        public SqlNode ElseExpression { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("CASE ");
            InputExpression.ToString(sb);
            sb.IncreaseIndent();
            
            foreach (var when in WhenExpressions)
            {
                sb.AppendLineAndIndent();
                when.ToString(sb);
            }
            if (ElseExpression != null)
            {
                sb.AppendLineAndIndent();
                sb.Append("ELSE ");
                ElseExpression.ToString(sb);
            }

            sb.DecreaseIndent();
            sb.AppendLineAndIndent();
            sb.Append("END");
        }
    }

    public class SqlCaseWhenNode: SqlNode
    {
        public SqlNode Condition { get; set; }
        public SqlNode Result { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("WHEN ");
            Condition.ToString(sb);
            sb.Append(" THEN ");
            Result.ToString(sb);
        }
    }
}
