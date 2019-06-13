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

        private SqlWithCteNode ParseCte(SqlTokenizer t)
        {
            // <identifier> ("(" <columnList> ")")? "AS" "(" <SelectStatement> ")"
            var name = t.Expect(SqlTokenType.Identifier);
            var cteNode = new SqlWithCteNode
            {
                Location = name.Location,
                Name = new SqlIdentifierNode(name)
            };

            var lookahead = t.Peek();
            if (lookahead.IsSymbol("("))
                cteNode.ColumnNames = ParseParenthesis(t, x => ParseList(x, ParseIdentifier)).Expression;

            t.Expect(SqlTokenType.Keyword, "AS");
            cteNode.Select = ParseParenthesis(t, ParseQueryExpression).Expression;
            return cteNode;
        }
    }
}
