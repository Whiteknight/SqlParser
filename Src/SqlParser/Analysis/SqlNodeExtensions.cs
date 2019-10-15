using System.Collections.Generic;
using System.Linq;
using SqlParser.Ast;

namespace SqlParser.Analysis
{
    public static class SqlNodeExtensions
    {
        public static IReadOnlyCollection<string> GetDataSources(this ISqlNode node)
        {
            return node.FindOfType<SqlObjectIdentifierNode>().Select(n => n.ToString()).ToList();
        }

        public static IReadOnlyCollection<string> GetVariableNames(this ISqlNode node)
        {
            return node.FindOfType<SqlVariableNode>().Select(n => n.Name).ToList();
        }
    }
}
