using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlIfNode ParseIf(SqlTokenizer t)
        {
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
