using System;
using System.Linq;
using ParserObjects;
using ParserObjects.Sequences;
using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
        // The Initialize methods are separated out into other files because of conflicting using
        // directives leading to confusion in the compiler
        private static readonly Lazy<IParser<char, SqlToken>> _lexer = new Lazy<IParser<char, SqlToken>>(InitializeLexer);
        private static readonly Lazy<IParser<SqlToken, ISqlNode>> _parser = new Lazy<IParser<SqlToken, ISqlNode>>(InitializeParser);

        // See, for some reference about differences from standard:
        // https://learnsql.com/blog/14-differences-sql-vs-tsql/

        public SqlStatementListNode Parse(string sql)
        {
            var lexer = _lexer.Value;
            var input = new StringCharacterSequence(sql);
            var tokens = lexer.ToSequence(input).Select(r => r.Value);

            var parser = _parser.Value;
            var result = parser.Parse(tokens);
            var unparsedRemainder = input.GetRemainder();
            return result.Value as SqlStatementListNode;
        }
    }
}
