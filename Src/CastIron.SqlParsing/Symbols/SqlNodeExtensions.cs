using CastIron.SqlParsing.Ast;

namespace CastIron.SqlParsing.Symbols
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
