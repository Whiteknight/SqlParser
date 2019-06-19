﻿using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser
{
    public partial class Parser
    {
        private SqlNode ParseInsertStatement(SqlTokenizer t)
        {
            // "INSERT" "INTO" <ObjectIdOrVariable> "(" <ColumnList> ")" <ValuesOrSelect>
            var insertToken = t.Expect(SqlTokenType.Keyword, "INSERT");
            t.Expect(SqlTokenType.Keyword, "INTO");

            var insertNode = new SqlInsertNode
            {
                Location = insertToken.Location
            };
            insertNode.Table = ParseObjectIdentifier(t);
            insertNode.Columns = ParseParenthesis(t, a => ParseList(a, ParseIdentifier)).Expression;

            // TODO: OUTPUT Clause

            var next = t.Peek();
            if (next.IsKeyword("VALUES"))
                insertNode.Source = ParseValues(t);
            else if (next.IsKeyword("SELECT"))
                insertNode.Source = ParseQueryExpression(t);
            else if (next.IsKeyword("DEFAULT"))
            {
                t.GetNext();
                t.Expect(SqlTokenType.Keyword, "VALUES");
                insertNode.Source = new SqlKeywordNode("DEFAULT VALUES");
            }

            return insertNode;
        }

        private SqlNode ParseValues(SqlTokenizer t)
        {
            // "VALUES" "(" <ValueList> ")" ("," "(" <ValueList> ")")*
            var valuesToken = t.Expect(SqlTokenType.Keyword, "VALUES");
            return new SqlValuesNode
            {
                Location = valuesToken.Location,
                Values = ParseList(t, a => ParseParenthesis(a, b => ParseList(b, ParseVariableOrConstant)).Expression)
            };
        }
    }
}