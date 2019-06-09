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

        public SqlStatementListNode(List<SqlNode> statements)
        {
            Statements = statements ?? new List<SqlNode>();
        }

        public override void ToString(SqlStringifier sb)
        {
            foreach (var statement in Statements)
            {
                statement.ToString(sb);
                sb.AppendLine(";");
            }
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitStatementList(this);

        public SqlStatementListNode Update(List<SqlNode> stmts)
        {
            if (stmts == Statements)
                return this;
            return new SqlStatementListNode(stmts)
            {
                Location = Location
            };
        }
    }
}