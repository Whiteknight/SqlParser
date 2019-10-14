using System.Collections.Generic;
using SqlParser.Ast;

namespace SqlParser.Analysis
{
    public static class SqlNodeExtensions
    {
        public static IReadOnlyCollection<SqlObjectIdentifierNode> GetDataSources(this ISqlNode node)
        {
            var names = new List<SqlObjectIdentifierNode>();
            var visitor = new GetNodesOfTypeAnalysisVisitor<SqlObjectIdentifierNode>(n => names.Add(n));
            visitor.Visit(node);
            return names;
        }

        public static IReadOnlyCollection<SqlVariableNode> GetVariableNames(this ISqlNode node)
        {
            var names = new List<SqlVariableNode>();
            var visitor = new GetNodesOfTypeAnalysisVisitor<SqlVariableNode>(n => names.Add(n));
            visitor.Visit(node);
            return names;
        }

        public static IEnumerable<ISqlNode> EnumerateAllNodes(this ISqlNode node)
        {
            // TODO: Need a way to do this incrementally, without having to load the whole
            // tree into a list
            var nodes = new List<ISqlNode>();
            var visitor = new GetNodesOfTypeAnalysisVisitor<ISqlNode>(n => nodes.Add(n));
            visitor.Visit(node);
            return nodes;
        }
    }
}
