using System.Collections.Generic;
using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStatementListNode : SqlNode
    {
        public List<SqlNode> Statements { get; }

        public SqlStatementListNode()
        {
            Statements = new List<SqlNode>();
        }

        public override void ToString(StringBuilder sb, int level)
        {
            foreach (var statement in Statements)
            {
                statement.ToString(sb, level);
                sb.AppendLine(";");
            }
        }
    }
}