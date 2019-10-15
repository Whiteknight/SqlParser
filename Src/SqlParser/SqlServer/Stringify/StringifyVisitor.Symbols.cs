using System.Collections.Generic;
using SqlParser.Ast;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.SqlServer.Stringify
{
    public partial class StringifyVisitor : ISqlNodeVisitor
    {
        private readonly Stack<SymbolTable> _allSymbolTables;
        private readonly Stack<SymbolTable> _nonNullSymbolTables;
        private SymbolTable _currentSymbolTable;

        public ISqlNode Visit(ISqlNode n)
        {
            if (n == null)
                return null;
            if (n is ISqlSymbolScopeNode scoped)
            {
                PushSymbolTable(scoped);
                var result = n.Accept(this);
                PopSymbolTable();
                return result;
            }

            n = _currentSymbolTable?.ResolveIdentifier(n) ?? n;
            return n.Accept(this);
        }

        private void PushSymbolTable(ISqlSymbolScopeNode n)
        {
            _allSymbolTables.Push(n.Symbols);
            if (n.Symbols != null)
            {
                _nonNullSymbolTables.Push(n.Symbols);
                _currentSymbolTable = n.Symbols;
            }
        }

        private void PopSymbolTable()
        {
            var st = _allSymbolTables.Pop();
            if (st != null && st == _nonNullSymbolTables.Peek())
                _currentSymbolTable = _nonNullSymbolTables.Pop();
        }
    }
}
