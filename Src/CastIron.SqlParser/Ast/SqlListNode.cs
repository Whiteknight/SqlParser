using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlListNode<TNode> : SqlNode, ICollection<TNode>
        where TNode : SqlNode
    {
        public SqlListNode()
        {
            Children = new List<TNode>();
        }

        public List<TNode> Children { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            foreach (var child in Children)
            {
                sb.AppendLine();
                sb.AppendIndent(level);
                child.ToString(sb, level);
            }
        }

        public IEnumerator<TNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Children).GetEnumerator();
        }

        public void Add(TNode item)
        {
            Children.Add(item);
        }

        public void Clear()
        {
            Children.Clear();
        }

        public bool Contains(TNode item)
        {
            return Children.Contains(item);
        }

        public void CopyTo(TNode[] array, int arrayIndex)
        {
            Children.CopyTo(array, arrayIndex);
        }

        public bool Remove(TNode item)
        {
            return Children.Remove(item);
        }

        public int Count => Children.Count;

        public bool IsReadOnly => false;
    }
}
