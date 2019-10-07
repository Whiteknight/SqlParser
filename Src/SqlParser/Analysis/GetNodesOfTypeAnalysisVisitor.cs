using System;
using SqlParser.Ast;

namespace SqlParser.Analysis
{
    public class GetNodesOfTypeAnalysisVisitor<TNode> : SqlNodeVisitor
        where TNode : SqlNode
    {
        private readonly Action<TNode> _onFound;

        // TODO: Predicate to satisfy before invoking callback
        // TODO: Subtree pruning (exclude some subtrees by match)
        public GetNodesOfTypeAnalysisVisitor(Action<TNode> onFound)
        {
            _onFound = onFound ?? throw new ArgumentNullException(nameof(onFound));
        }

        public override SqlNode Visit(SqlNode n)
        {
            if (n is TNode match)
                _onFound(match);
            return base.Visit(n);
        }
    }
}
