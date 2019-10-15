using System.Collections.Generic;
using SqlParser.Ast;
using SqlParser.Symbols;

namespace SqlParser.SqlServer.Symbols
{
    public static class SqlNodeExtensions
    {
        public static void BuildSymbolTables(this ISqlNode n, IEnumerable<SymbolInfo> environmentalSymbols = null)
        {
            if (n == null)
                return;
            new SymbolTableBuildVisitor(environmentalSymbols).Visit(n);
        }
    }
}
