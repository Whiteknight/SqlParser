using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser
{
    public partial class Parser
    {
        public SqlMergeNode ParseMergeStatement(Tokenizer t)
        {
            var mergeToken = t.Expect(SqlTokenType.Keyword, "MERGE");
            var mergeNode = new SqlMergeNode
            {
                Location = mergeToken.Location
            };
            // TODO: "TOP" <maybeParenVariableOrNumericExpression> "PERCENT"?
            
            t.NextIs(SqlTokenType.Keyword, "INTO", true);
            mergeNode.Target = ParseMaybeAliasedTable(t, ParseObjectIdentifier);
            t.Expect(SqlTokenType.Keyword, "USING");
            mergeNode.Source = ParseMaybeAliasedTable(t, ParseObjectIdentifier);
            t.Expect(SqlTokenType.Keyword, "ON");
            mergeNode.MergeCondition = ParseBooleanExpression(t);
            while(true)
            {
                var whenClauseToken = t.MaybeGetKeywordSequence("WHEN", "NOT", "MATCHED", "BY", "SOURCE", "TARGET");
                if (whenClauseToken == null)
                    break;
                // TODO: "AND" <clauseSearchCondition>
                t.Expect(SqlTokenType.Keyword, "THEN");
                if (whenClauseToken.Value == "WHEN MATCHED")
                {
                    // TODO: Allow multiple
                    mergeNode.Matched = ParseMergeMatched(t);
                }
                else if (whenClauseToken.Value == "WHEN NOT MATCHED" || whenClauseToken.Value == "WHEN NOT MATCHED BY TARGET")
                {
                    mergeNode.NotMatchedByTarget = ParseMergeNotMatched(t);
                }
                else if (whenClauseToken.Value == "WHEN NOT MATCHED BY SOURCE")
                {
                    // TODO: Allow multiple
                    mergeNode.NotMatchedBySource = ParseMergeMatched(t);
                }
            }
            // TODO: Output clause
            // TODO: OPTION clause
            return mergeNode;
        }

        public SqlNode ParseMergeMatched(Tokenizer t)
        {
            if (t.NextIs(SqlTokenType.Keyword, "UPDATE"))
            {
                var updateToken = t.Expect(SqlTokenType.Keyword, "UPDATE");
                var setList = ParseUpdateSetClause(t);
                return new SqlMergeUpdateNode
                {
                    Location = updateToken.Location,
                    SetClause = setList
                };
            }
            if (t.NextIs(SqlTokenType.Keyword, "DELETE"))
            {
                var deleteToken = t.Expect(SqlTokenType.Keyword, "DELETE");
                return new SqlKeywordNode(deleteToken);
            }

            throw ParsingException.CouldNotParseRule(nameof(ParseMergeMatched), t.Peek());
        }

        public SqlNode ParseMergeNotMatched(Tokenizer t)
        {
            var insertToken = t.Expect(SqlTokenType.Keyword, "INSERT");
            var insertNode = new SqlMergeInsertNode
            {
                Location = insertToken.Location,
                Columns = ParseInsertColumnList(t)
            };

            var next = t.Peek();
            if (next.IsKeyword("VALUES"))
                insertNode.Source = ParseValues(t);
            else if (next.IsKeyword("DEFAULT"))
            {
                t.GetNext();
                t.Expect(SqlTokenType.Keyword, "VALUES");
                insertNode.Source = new SqlKeywordNode("DEFAULT VALUES");
            }
            else
                throw new ParsingException("INSERT statement does not have a source");

            return insertNode;
        }
    }
}
