﻿using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser
{
    public partial class Parser
    {
        private SqlNode ParseDeleteStatement(Tokenizer t)
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
