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
            with.Ctes = ParseList(t, ParseCte);
            var statement = ParseStatement(t);
            with.Statement = statement;
            return with;
        }

        private SqlCteNode ParseCte(SqlTokenizer t)
        {
            // <identifier> ("(" <columnList> ")")? "AS"? "(" <SelectStatement> ")"
            var name = t.Expect(SqlTokenType.Identifier);
            // TODO: ColumnList
            t.NextIs(SqlTokenType.Keyword, "AS", true);
            var selectStatement = ParseParenthesis(t, ParseQueryExpression).Expression;
            return new SqlCteNode
            {
                Name = new SqlIdentifierNode { Name = name.Value },
                Select = selectStatement
            };
        }
    }
}
