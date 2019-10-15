using System;
using System.Collections.Generic;
using SqlParser.Ast;

namespace SqlParser.Visiting
{
    public class GetNodesVisitor : SqlNodeVisitor
    {
        private readonly Func<ISqlNode, bool> _predicate;
        private readonly ICollection<ISqlNode> _found;

        public GetNodesVisitor(Func<ISqlNode, bool> predicate, ICollection<ISqlNode> found)
        {
            _predicate = predicate;
            _found = found;
        }

        public static IReadOnlyCollection<ISqlNode> OfType<T>(ISqlNode root)
            where T : class, ISqlNode
        {
            var list = new List<ISqlNode>();
            new GetNodesVisitor(n => n is T, list).Visit(root);
            return list;
        }

        public override ISqlNode Visit(ISqlNode n)
        {
            if (_predicate(n))
                _found.Add(n);
            return base.Visit(n);
        }
    }
}