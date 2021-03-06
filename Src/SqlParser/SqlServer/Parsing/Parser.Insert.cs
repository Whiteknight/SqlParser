﻿using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseInsertStatement(ITokenizer t)
        {
            // "INSERT" "INTO" <ObjectIdOrVariable> "(" <ColumnList> ")" <ValuesOrSelect>
            var insertToken = t.Expect(SqlTokenType.Keyword, "INSERT");
            t.Expect(SqlTokenType.Keyword, "INTO");

            var insertNode = new SqlInsertNode
            {
                Location = insertToken.Location,
                Table = ParseObjectIdentifier(t),
                Columns = ParseInsertColumnList(t)
            };

            // TODO: OUTPUT Clause

            var next = t.Peek();
            if (next.IsKeyword("VALUES"))
                insertNode.Source = ParseValues(t);
            else if (next.IsKeyword("SELECT"))
                insertNode.Source = ParseQueryExpression(t);
            else if (next.IsKeyword("EXEC", "EXECUTE"))
                insertNode.Source = ParseExecute(t);
            else if (next.IsKeyword("DEFAULT"))
            {
                t.GetNext();
                t.Expect(SqlTokenType.Keyword, "VALUES");
                insertNode.Source = new SqlKeywordNode("DEFAULT VALUES");
            }
            else
                throw new ParsingException("INSERT INTO statement does not have a source");

            return insertNode;
        }

        private SqlListNode<SqlIdentifierNode> ParseInsertColumnList(ITokenizer t)
        {
            return ParseParenthesis(t, a => ParseList(a, ParseIdentifier)).Expression;
        }

        private ISqlNode ParseValues(ITokenizer t)
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
