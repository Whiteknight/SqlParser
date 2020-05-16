using System;
using SqlParser.Ast;
using SqlParser.Visiting;

namespace SqlParser.Analysis
{
    public class SubtreeVisitor : SqlNode, ISqlNodeVisitor
    {
        private readonly ISqlNodeVisitor _inner;
        private readonly Predicate<ISqlNode> _include;

        public SubtreeVisitor(ISqlNodeVisitor inner, Predicate<ISqlNode> include)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _include = include ?? (n => true);
        }

        public ISqlNode Visit(ISqlNode n)
        {
            if (_include(n))
                return _inner.Visit(n);
            return n;
        }
    }
}