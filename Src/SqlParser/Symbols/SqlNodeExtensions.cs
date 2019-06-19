using SqlParser.Ast;

namespace SqlParser.Symbols
{
    public static class SqlNodeExtensions
    {
        public static void BuildSymbolTables(this SqlNode n)
        {
            if (n == null)
                return;
            new SymbolTableBuildVisitor().Visit(n);
        }
    }
}
