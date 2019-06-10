using System.Collections;
using System.Collections.Generic;
using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStatementListNode : SqlNode, ISqlSymbolScopeNode, ICollection<SqlNode>
    {
        public List<SqlNode> Statements { get; }
        public SymbolTable Symbols { get; set; }

        public SqlStatementListNode()
        {
            Statements = new List<SqlNode>();
        }

        public SqlStatementListNode(List<SqlNode> statements)
        {
            Statements = statements ?? new List<SqlNode>();
        }

        public override void ToString(SqlStringifier sb)
        {
            foreach (var statement in Statements)
            {
                statement.ToString(sb);
                sb.AppendLine(";");
            }
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitStatementList(this);

        public SqlStatementListNode Update(List<SqlNode> stmts)
        {
            if (stmts == Statements)
                return this;
            return new SqlStatementListNode(stmts)
            {
                Location = Location
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
    }
}