using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
        private SqlIfNode ParseIf(Tokenizer t)
        {
            // "IF" (("(" <Condition> ")") | <Condition>) <Statement> ("ELSE" <Statement>)?
            var ifToken = t.Expect(SqlTokenType.Keyword, "IF");
            var ifNode = new SqlIfNode
            {
                Location = ifToken.Location,
                Condition = ParseMaybeParenthesis(t, ParseBooleanExpression),
                Then = ParseStatement(t)
            };
            
            if (t.NextIs(SqlTokenType.Keyword, "ELSE", true))
                ifNode.Else = ParseStatement(t);
            return ifNode;
        }
    }
}
