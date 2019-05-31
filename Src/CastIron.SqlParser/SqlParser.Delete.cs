using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseDeleteStatement(SqlTokenizer t)
        {
            var deleteToken = t.Expect(SqlTokenType.Keyword, "DELETE");
            // TODO: TOP clause
            t.NextIs(SqlTokenType.Keyword, "FROM", true);
            var table = ParseObjectIdentifier(t);
            var where = ParseWhereClause(t);
            // TODO: OUTPUT clause
            return new SqlDeleteNode
            {
                Location = deleteToken.Location,
                Source = table,
                WhereClause = where
            };
        }
    }
}
