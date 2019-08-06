using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser
{
    public partial class Parser
    {
        private SqlNode ParseInsertStatement(Tokenizer t)
        {
            // "INSERT" "INTO" <ObjectIdOrVariable> "(" <ColumnList> ")" <ValuesOrSelect>
            var insertToken = t.Expect(SqlTokenType.Keyword, "INSERT");
            t.Expect(SqlTokenType.Keyword, "INTO");

            var insertNode = new SqlInsertNode
            {
                Location = insertToken.Location
            };
            insertNode.Table = ParseObjectIdentifier(t);
            insertNode.Columns = ParseInsertColumnList(t);

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

        private SqlListNode<SqlIdentifierNode> ParseInsertColumnList(Tokenizer t)
        {
            return ParseParenthesis(t, a => ParseList(a, ParseIdentifier)).Expression;
        }

        private SqlNode ParseValues(Tokenizer t)
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
