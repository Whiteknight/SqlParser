using SqlParser.Ast;
using SqlParser.SqlServer.Symbols;

namespace SqlParser.Symbols
{
    public static class SqlNodeExtensions
    {
        public static void BuildSymbolTables(this ISqlNode n)
        {
            if (n == null)
                return;
            new SymbolTableBuildVisitor().Visit(n);
        }
    }
}
