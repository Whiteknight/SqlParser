using SqlParser.Ast;

namespace SqlParser.Tests.Utility
{
    public static class SqlNodeExtensions
    {
        public static SqlNodeAssertions Should(this SqlNode node)
        {
            return new SqlNodeAssertions(node);
        }
    }
}