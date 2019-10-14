using System.Text;
using SqlParser.Ast;

namespace SqlParser.PostgreSql.Stringify
{
    public static class SqlNodeExtensions
    {
        public static string ToPostgresString(this ISqlNode n)
        {
            var sb = new StringBuilder();
            var visitor = new StringifyVisitor(sb);
            visitor.Visit(n);
            return sb.ToString();
        }
    }
}
