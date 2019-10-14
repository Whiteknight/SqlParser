using System.Text;
using SqlParser.Ast;

namespace SqlParser.SqlServer.Stringify
{
    public static class SqlNodeExtensions
    {
        public static string ToSqlServerString(this ISqlNode n)
        {
            var sb = new StringBuilder();
            var visitor = new StringifyVisitor(sb);
            visitor.Visit(n);
            return sb.ToString();
        }
    }
}
