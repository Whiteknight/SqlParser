using System.Collections.Generic;
using SqlParser.Ast;

namespace SqlParser.Analysis
{
    public class GetNodesOfTypeAnalysisVisitor<TNode> : SqlNodeVisitor
        where TNode : SqlNode
    {
        private readonly List<TNode> _nodes;

        public GetNodesOfTypeAnalysisVisitor()
        {
            _nodes = new List<TNode>();
        }

        public IReadOnlyList<TNode> GetNodes() => _nodes;

        public override SqlNode Visit(SqlNode n)
        {
            if (n is TNode match)
                _nodes.Add(match);
            return base.Visit(n);
        }
    }
}
