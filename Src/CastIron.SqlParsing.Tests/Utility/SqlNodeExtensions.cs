using CastIron.SqlParsing.Ast;

namespace CastIron.SqlParsing.Tests.Utility
{
    public static class SqlNodeExtensions
    {
        public static SqlNodeAssertions Should(this SqlNode node)
        {
            return new SqlNodeAssertions(node);
        }
    }
}