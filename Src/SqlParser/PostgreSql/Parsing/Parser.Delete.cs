﻿using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseDeleteStatement(ITokenizer t)
        {
            var deleteToken = t.Expect(SqlTokenType.Keyword, "DELETE");
            // TODO: TOP clause
            t.NextIs(SqlTokenType.Keyword, "FROM", true);
            var table = ParseObjectIdentifier(t);
            var where = ParseWhereClause(t);
            // TODO: RETURNING clause
            return new SqlDeleteNode
            {
                Location = deleteToken.Location,
                Source = table,
                WhereClause = where
            };
        }
    }
}
