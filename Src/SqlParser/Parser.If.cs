using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser
{
    public partial class Parser
    {
        private SqlIfNode ParseIf(SqlTokenizer t)
        {
            // "IF" (("(" <Condition> ")") | <Condition>) <Statement> ("ELSE" <Statement>)?
            var ifToken = t.Expect(SqlTokenType.Keyword, "IF");
            var condition = ParseMaybeParenthesis(t, ParseBooleanExpression);
            var then = ParseStatement(t);
            var ifNode = new SqlIfNode
            {
                Location = ifToken.Location,
                Condition = condition,
                Then = then
            };
            if (t.NextIs(SqlTokenType.Keyword, "ELSE", true))
                ifNode.Else = ParseStatement(t);
            return ifNode;
        }
    }
}
