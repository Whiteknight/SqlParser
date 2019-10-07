using System.Collections.Generic;
using SqlParser.Ast;

namespace SqlParser.Analysis
{
    public static class SqlNodeExtensions
    {
        public static IReadOnlyCollection<string> GetDataSources(this SqlNode node)
        {
            var names = new List<string>();
            var visitor = new GetNodesOfTypeAnalysisVisitor<SqlObjectIdentifierNode>(n => names.Add(n.ToString()));
            visitor.Visit(node);
            return names;
        }

        public static IReadOnlyCollection<string> GetVariableNames(this SqlNode node)
        {
            var names = new List<string>();
            var visitor = new GetNodesOfTypeAnalysisVisitor<SqlVariableNode>(n => names.Add(n.ToString()));
            visitor.Visit(node);
            return names;
        }

        public static IEnumerable<SqlNode> EnumerateAllNodes(this SqlNode node)
        {
            // TODO: Need a way to do this incrementally, without having to load the whole
            // tree into a list
            var nodes = new List<SqlNode>();
            var visitor = new GetNodesOfTypeAnalysisVisitor<SqlNode>(n => nodes.Add(n));
            visitor.Visit(node);
            return nodes;
        }
    }
}
