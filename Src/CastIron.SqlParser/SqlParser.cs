using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        public SqlNode Parse(SqlTokenizer t)
        {
            return ParseStatementList(t);
        }

        private SqlNode ParseStatementList(SqlTokenizer t)
        {
            var statements = new SqlStatementListNode();
            while (true)
            {
                var statement = ParseStatement(t);
                statements.Statements.Add(statement);
                t.NextIs(SqlTokenType.Symbol, ";", true);
                if (t.Peek().IsType(SqlTokenType.EndOfInput))
                    break;
            }

            if (statements.Statements.Count == 0)
                return null;
            return statements;
        }

        private SqlNode ParseStatement(SqlTokenizer t)
        {
            t.Skip(SqlTokenType.Whitespace);
            var keyword = t.ExpectPeek(SqlTokenType.Keyword);
            if (keyword.Value == "SELECT")
                return ParseQueryExpression(t);
            if (keyword.Value == "WITH")
                return ParseWithStatement(t);
            if (keyword.Value == "INSERT")
                return ParseInsertStatement(t);
            // ...
            throw new Exception($"Cannot parse statement starting with {keyword}");
        }
    }
}
