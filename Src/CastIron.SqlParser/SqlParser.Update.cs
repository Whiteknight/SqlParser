using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseUpdateStatement(SqlTokenizer t)
        {
            var updateToken = t.Expect(SqlTokenType.Keyword, "UPDATE");
            // TODO: TOP clause
            var table = ParseObjectIdentifier(t);
            t.Expect(SqlTokenType.Keyword, "SET");
            var setList = ParseList(t, ParseUpdateColumnAssignExpression);
            // TODO: OUTPUT clause
            var where = ParseWhereClause(t);
            return new SqlUpdateNode
            {
                Location = updateToken.Location,
                Source = table,
                SetClause = setList,
                WhereClause = where
            };
        }
    }
}
