using System;
using System.Collections;
using System.Collections.Generic;

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

        public override void ToString(SqlStringifier sb)
        {
            void between(SqlStringifier x)
            {
                x.AppendLineAndIndent(",");
            }
            void forEach(SqlStringifier x, TNode child)
            {
                child.ToString(x);
            }

            ToString(sb, forEach, between);
        }

        public void ToString(SqlStringifier sb, Action<SqlStringifier, TNode> forEach, Action<SqlStringifier> between)
        {
            if (Children.Count == 0)
                return;
            forEach?.Invoke(sb, Children[0]);
            for (int i = 1; i < Children.Count; i++)
            {
                between?.Invoke(sb);
                forEach?.Invoke(sb, Children[i]);
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
