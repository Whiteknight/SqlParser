using System.Collections.Generic;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStatementListNode : SqlNode
    {
        public List<SqlNode> Statements { get; }

        public SqlStatementListNode()
        {
            Statements = new List<SqlNode>();
        }

        public override void ToString(SqlStringifier sb)
        {
            foreach (var statement in Statements)
            {
                statement.ToString(sb);
                sb.AppendLine(";");
            }
        }
    }
}