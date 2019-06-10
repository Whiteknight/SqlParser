using System.Collections.Generic;
using CastIron.SqlParsing.Ast;

namespace CastIron.SqlParsing.Symbols
{
    public class SymbolTableBuildVisitor : SqlNodeVisitor
    {
        private readonly Stack<SymbolTable> _tables;

        public SymbolTableBuildVisitor()
        {
            _tables = new Stack<SymbolTable>();
            _tables.Push(new SymbolTable());
        }

        private SymbolTable PushSymbolTable()
        {
            var table = new SymbolTable(_tables.Peek());
            _tables.Push(table);
            return table;
        }

        private void PopSymbolTable()
        {
            _tables.Pop();
        }

        private SymbolTable Current => _tables.Peek();

        public override SqlNode VisitCte(SqlCteNode n)
        {
            Current.AddSymbol(n.Name.Name, new SymbolInfo { DataType = "TableExpression", DefinedAt = n.Location });
            return base.VisitCte(n);
        }

        public override SqlNode VisitDeclare(SqlDeclareNode n)
        {
            Current.AddSymbol(n.Variable.Name, new SymbolInfo { DataType = "Variable", DefinedAt = n.Location });
            return base.VisitDeclare(n);
        }

        public override SqlNode VisitDelete(SqlDeleteNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitDelete(n) as SqlDeleteNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override SqlNode VisitFrom(SqlSelectFromClauseNode n)
        {
            // Identifier is presumed to be table name
            AddTableIds(n.Source);
            
            // table variables should already be defined
            if (n.Source is SqlVariableNode v)
                Current.GetInfoOrThrow(v.Name);

            // base will visit subexpressions, such as Joins
            return base.VisitFrom(n);
        }

        public override SqlJoinNode VisitJoin(SqlJoinNode n)
        {
            AddTableIds(n.Left);
            AddTableIds(n.Right);
            return base.VisitJoin(n);
        }

        private void AddTableIds(SqlNode n)
        {
            // Identifier is presumed to be table name
            if (n is SqlIdentifierNode id)
                Current.AddSymbol(id.Name, new SymbolInfo { DataType = "TableExpression", DefinedAt = id.Location });

            // Object ID is a table name, only use the name of the table as the symbol, not the full qualification
            if (n is SqlObjectIdentifierNode objId)
                Current.AddSymbol(objId.Name.Name, new SymbolInfo { DataType = "TableExpression", DefinedAt = objId.Location });

            if (n is SqlAliasNode alias)
                Current.AddSymbol(alias.Alias.Name, new SymbolInfo { DataType = "TableExpression", DefinedAt = alias.Location });
        }

        public override SqlNode VisitSelect(SqlSelectNode n)
        {
            var symbols = PushSymbolTable();
            // Visit the FROM clause to get all the source table expressions
            Visit(n.FromClause);

            // Visit the columns to get all column names and aliases
            foreach (var column in n.Columns)
            {
                // TODO: Make sure all symbols in the column definitions are defined
                if (column is SqlAliasNode alias)
                    Current.AddSymbol(alias.Alias.Name, new SymbolInfo { DataType = "Column", DefinedAt = alias.Location });

                // Identifiers are presumed to be column names from tables in the FROM list
                if (column is SqlIdentifierNode id)
                    Current.AddSymbol(id.Name, new SymbolInfo { DataType = "Column", DefinedAt = id.Location });

                // Qualified IDs are presumed <tableOrAlias>.<column>
                if (column is SqlQualifiedIdentifierNode qid)
                {
                    // Make sure the table name is already defined in the FROM clause
                    Current.GetInfoOrThrow(qid.Qualifier.Name);
                    // We want <table>.<column>, not <table>.*
                    if (qid.Identifier is SqlIdentifierNode nested)
                        Current.AddSymbol(nested.Name, new SymbolInfo { DataType = "Column", DefinedAt = qid.Location });
                }

                // Variable should already be defiend
                if (column is SqlVariableNode v)
                    Current.GetInfoOrThrow(v.Name);
            }

            // TODO: Visit the ORDER BY, GROUP BY, WHERE and HAVING clauses to make sure all symbols there are defined

            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override SqlNode VisitSet(SqlSetNode n)
        {
            Current.GetInfoOrThrow(n.Variable.Name);
            return base.VisitSet(n);
        }

        public override SqlNode VisitStatementList(SqlStatementListNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitStatementList(n) as SqlStatementListNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override SqlNode VisitUpdate(SqlUpdateNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitUpdate(n) as SqlUpdateNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override SqlNode VisitWith(SqlWithNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitWith(n) as SqlWithNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        
    }
}
