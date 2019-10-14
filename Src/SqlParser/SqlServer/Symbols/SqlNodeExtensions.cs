using SqlParser.Ast;

namespace SqlParser.SqlServer.Symbols
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
