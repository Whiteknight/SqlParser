using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseWithStatement(Tokenizer t)
        {
            // "WITH" <Cte> ("," <Cte>)* <WithChildStatement>
            var with = new SqlWithNode();
            t.Expect(SqlTokenType.Keyword, "WITH");
            with.Ctes = ParseList(t, ParseCte);
            var statement = ParseWithChildStatement(t);
            with.Statement = statement;
            return with;
        }

        private SqlWithCteNode ParseCte(Tokenizer t)
        {
            // "RECURSIVE"? <identifier> ("(" <columnList> ")")? "AS" "(" <QueryExpression> ")"
            bool isRecursive = t.NextIs(SqlTokenType.Keyword, "RECURSIVE", true);
            var name = t.Expect(SqlTokenType.Identifier);
            var cteNode = new SqlWithCteNode
            {
                Location = name.Location,
                Name = new SqlIdentifierNode(name),
                Recursive = isRecursive
            };

            var lookahead = t.Peek();
            if (lookahead.IsSymbol("("))
                cteNode.ColumnNames = ParseParenthesis(t, x => ParseList(x, ParseIdentifier)).Expression;

            t.Expect(SqlTokenType.Keyword, "AS");
            cteNode.Select = ParseParenthesis(t, ParseQueryExpression).Expression;
            return cteNode;
        }

        private ISqlNode ParseWithChildStatement(Tokenizer t)
        {
            while (t.NextIs(SqlTokenType.Symbol, ";", true)) ;
            var stmt = ParseUnterminatedWithChildStatement(t);
            t.NextIs(SqlTokenType.Symbol, ";", true);
            return stmt;
        }

        private ISqlNode ParseUnterminatedWithChildStatement(Tokenizer t)
        {
            t.Skip(SqlTokenType.Whitespace);

            var keyword = t.ExpectPeek(SqlTokenType.Keyword);
            if (keyword.Value == "SELECT")
                return ParseQueryExpression(t);
            if (keyword.Value == "INSERT")
                return ParseInsertStatement(t);
            if (keyword.Value == "UPDATE")
                return ParseUpdateStatement(t);
            if (keyword.Value == "DELETE")
                return ParseDeleteStatement(t);
            if (keyword.Value == "MERGE")
                return ParseMergeStatement(t);

            return null;
        }
    }
}
