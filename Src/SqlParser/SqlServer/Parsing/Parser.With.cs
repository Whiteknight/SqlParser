﻿using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseWithStatement(ITokenizer t)
        {
            // "WITH" <Cte> ("," <Cte>)* <WithChildStatement>
            var with = new SqlWithNode();
            t.Expect(SqlTokenType.Keyword, "WITH");
            with.Ctes = ParseList(t, ParseCte);
            var statement = ParseWithChildStatement(t);
            with.Statement = statement;
            return with;
        }

        private SqlWithCteNode ParseCte(ITokenizer t)
        {
            // <identifier> ("(" <columnList> ")")? "AS" "(" <QueryExpression> ")"
            var name = t.Expect(SqlTokenType.Identifier);
            var cteNode = new SqlWithCteNode
            {
                Location = name.Location,
                Name = new SqlIdentifierNode(name),
                Recursive = false
            };

            var lookahead = t.Peek();
            if (lookahead.IsSymbol("("))
                cteNode.ColumnNames = ParseParenthesis(t, x => ParseList(x, ParseIdentifier)).Expression;

            t.Expect(SqlTokenType.Keyword, "AS");
            cteNode.Select = ParseParenthesis(t, ParseQueryExpression).Expression;
            cteNode.DetectRecursion();
            return cteNode;
        }

        private ISqlNode ParseWithChildStatement(ITokenizer t)
        {
            while (t.NextIs(SqlTokenType.Symbol, ";", true)) ;
            var stmt = ParseUnterminatedWithChildStatement(t);
            t.NextIs(SqlTokenType.Symbol, ";", true);
            return stmt;
        }

        private ISqlNode ParseUnterminatedWithChildStatement(ITokenizer t)
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
