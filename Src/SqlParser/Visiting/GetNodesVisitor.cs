using System;
using System.Collections.Generic;

namespace SqlParser.Visiting
{
    public class GetNodesVisitor : SqlNodeVisitor
    {
        private readonly Func<ISqlNode, bool> _predicate;
        private readonly ICollection<ISqlNode> _found;
        private readonly int _limit;

        public GetNodesVisitor(Func<ISqlNode, bool> predicate, ICollection<ISqlNode> found, int limit = 0)
        {
            _predicate = predicate;
            _found = found;
            _limit = limit;
        }

        public override ISqlNode Visit(ISqlNode n)
        {
            if (_limit > 0 && _found.Count >= _limit)
                return n;
            if (_predicate(n))
            {
                _found.Add(n);
                if (_limit > 0 && _found.Count >= _limit)
                    return n;
            }

            return base.Visit(n);
        }
    }
}