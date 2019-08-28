using System.Collections;
using System.Collections.Generic;
using SqlParser.Symbols;

namespace SqlParser.Ast
{
    public class SqlStatementListNode : SqlNode, ISqlSymbolScopeNode, IList<SqlNode>
    {
        public List<SqlNode> Statements { get; }
        public bool UseBeginEnd { get; set; }
        public SymbolTable Symbols { get; set; }
        

        public SqlStatementListNode()
        {
            Statements = new List<SqlNode>();
        }

        public SqlStatementListNode(List<SqlNode> statements)
        {
            Statements = statements ?? new List<SqlNode>();
        }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitStatementList(this);

        public SqlStatementListNode Update(List<SqlNode> stmts, bool isBeginEnd)
        {
            if (stmts == Statements && isBeginEnd == UseBeginEnd)
                return this;
            return new SqlStatementListNode(stmts)
            {
                Location = Location,
                UseBeginEnd = isBeginEnd
            };
        }

        public IEnumerator<SqlNode> GetEnumerator()
        {
            return Statements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(SqlNode item)
        {
            Statements.Add(item);
        }

        public void Clear()
        {
            Statements.Clear();
        }

        public bool Contains(SqlNode item)
        {
            return Statements.Contains(item);
        }

        public void CopyTo(SqlNode[] array, int arrayIndex)
        {
            Statements.CopyTo(array, arrayIndex);
        }

        public bool Remove(SqlNode item)
        {
            return Statements.Remove(item);
        }

        public int Count => Statements.Count;
        public bool IsReadOnly => false;
        public int IndexOf(SqlNode item)
        {
            return Statements.IndexOf(item);
        }

        public void Insert(int index, SqlNode item)
        {
            Statements.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Statements.RemoveAt(index);
        }

        public SqlNode this[int index]
        {
            get => Statements[index];
            set => Statements[index] = value;
        }
    }
}