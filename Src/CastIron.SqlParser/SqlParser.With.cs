using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseWithStatement(SqlTokenizer t)
        {
            // "WITH" <CTE> ("," <CTE>)* <Statement>
            var with = new SqlWithNode();
            t.Expect(SqlTokenType.Keyword, "WITH");
            while (true)
            {
                var cte = ParseCte(t);
                with.Ctes.Add(cte);
                if (t.NextIs(SqlTokenType.Symbol, ",", true))
                    continue;
                break;
            }

            var statement = ParseStatement(t);
            with.Statement = statement;
            return with;
        }

        private SqlCteNode ParseCte(SqlTokenizer t)
        {
            // <identifier> "AS"? "(" <SelectStatement> ")"
            var name = t.Expect(SqlTokenType.Identifier);
            t.NextIs(SqlTokenType.Keyword, "AS", true);
            t.Expect(SqlTokenType.Symbol, "(");
            var selectStatement = ParseQueryExpression(t);
            t.Expect(SqlTokenType.Symbol, ")");
            return new SqlCteNode
            {
                Name = new SqlIdentifierNode { Name = name.Value },
                Select = selectStatement
            };
        }
    }
}
