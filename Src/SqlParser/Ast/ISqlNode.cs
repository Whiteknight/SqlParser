using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public interface ISqlNode
    {
        Location Location { get; set; }
        ISqlNode Accept(INodeVisitorTyped visitor);
    }

    public static class SqlNodeExtensions
    {
        public static IReadOnlyCollection<ISqlNode> Find(this ISqlNode root, Func<ISqlNode, bool> predicate)
        {
            var list = new List<ISqlNode>();
            new GetNodesVisitor(predicate, list).Visit(root);
            return list;
        }

        public static IEnumerable<T> FindOfType<T>(this ISqlNode root)
            where T : class, ISqlNode
        {
            return root.Find(n => n is T).Cast<T>();
        }

        public static IReadOnlyCollection<ISqlNode> AllNodes(this ISqlNode root)
        {
            return root.Find(n => true);
        }

        public static string ToSqlServerString(this ISqlNode n)
        {
            var sb = new StringBuilder();
            new SqlServer.Stringify.StringifyVisitor(sb).Visit(n);
            return sb.ToString();
        }

        public static string ToPostgreSqlString(this ISqlNode n)
        {
            var sb = new StringBuilder();
            new PostgreSql.Stringify.StringifyVisitor(sb).Visit(n);
            return sb.ToString();
        }
    }
}
