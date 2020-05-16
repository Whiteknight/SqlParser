using System.Collections;
using System.Collections.Generic;
using ParserObjects;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlStatementListNode : SqlNode, ISqlNode, ISqlSymbolScopeNode, IList<ISqlNode>
    {
        public List<ISqlNode> Statements { get; }
        public bool UseBeginEnd { get; set; }
        public SymbolTable Symbols { get; set; }
        

        public SqlStatementListNode()
        {
            Statements = new List<ISqlNode>();
        }

        public SqlStatementListNode(List<ISqlNode> statements)
        {
            Statements = statements ?? new List<ISqlNode>();
        }

        public Location Location { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitStatementList(this);

        public SqlStatementListNode Update(List<ISqlNode> stmts, bool isBeginEnd)
        {
            if (stmts == Statements && isBeginEnd == UseBeginEnd)
                return this;
            return new SqlStatementListNode(stmts)
            {
                Location = Location,
                UseBeginEnd = isBeginEnd
            };
        }

        public IEnumerator<ISqlNode> GetEnumerator()
        {
            return Statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ISqlNode item)
        {
            Statements.Add(item);
        }

        public void Clear()
        {
            Statements.Clear();
        }

        public bool Contains(ISqlNode item)
        {
            return Statements.Contains(item);
        }

        public void CopyTo(ISqlNode[] array, int arrayIndex)
        {
            Statements.CopyTo(array, arrayIndex);
        }

        public bool Remove(ISqlNode item)
        {
            return Statements.Remove(item);
        }

        public int Count => Statements.Count;
        public bool IsReadOnly => false;
        public int IndexOf(ISqlNode item)
        {
            return Statements.IndexOf(item);
        }

        public void Insert(int index, ISqlNode item)
        {
            Statements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Statements.RemoveAt(index);
        }

        public ISqlNode this[int index]
        {
            get => Statements[index];
            set => Statements[index] = value;
        }
    }
}