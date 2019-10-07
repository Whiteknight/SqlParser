using System;
using SqlParser.Ast;

namespace SqlParser.Analysis
{
    public class SubtreeVisitor : ISqlNodeVisitor
    {
        private readonly ISqlNodeVisitor _inner;
        private readonly Predicate<SqlNode> _include;

        public SubtreeVisitor(ISqlNodeVisitor inner, Predicate<SqlNode> include)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            _include = include ?? (n => true);
        }

        public SqlNode Visit(SqlNode n)
        {
            if (_include(n))
                return _inner.Visit(n);
            return n;
        }
    }
}