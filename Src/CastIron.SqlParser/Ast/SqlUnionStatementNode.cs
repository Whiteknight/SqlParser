using System.Collections.Generic;
using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlUnionStatementNode : SqlNode
    {
        public SqlNode First { get; set; }
        public string Operator { get; set; }
        public SqlNode Second { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            First.ToString(sb, level);
            sb.AppendLine();
            sb.AppendIndent(level);
            sb.AppendLine(Operator);
            Second.ToString(sb, level);
        }
    }
}