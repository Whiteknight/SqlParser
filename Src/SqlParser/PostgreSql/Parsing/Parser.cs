using System;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;
using SqlParser.Ast;
using SqlParser.Tokenizing;
using static ParserObjects.Parsers.ParserMethods<char>;

namespace SqlParser.PostgreSql.Parsing
{
    public class Parser
    {
        private static readonly Lazy<IParser<char, SqlToken>> _lexer = new Lazy<IParser<char, SqlToken>>(InitializeLexer);
        private static readonly Lazy<IParser<SqlToken, ISqlNode>> _parser = new Lazy<IParser<SqlToken, ISqlNode>>(InitializeParser);

        private static IParser<SqlToken, ISqlNode> InitializeParser()
        {
            return new SqlStandard.SqlStandardGrammar().Parser;
        }

        private static IParser<char, SqlToken> InitializeLexer()
        {
            var lexer = SqlStandard.LexicalGrammar.CreateParser();
            var idStart = Match(c => char.IsLetter(c) || c == '_');
            var idPart = Match(c => char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '$' || c == '@' || c == '#');
            var regularIdentifier = Rule(
                    idStart,
                    idPart.List().Transform(c => c.ToArray()),

                    (start, parts) => start + new string(parts)
                )
                .Transform(c =>
                {
                    if (Facts.IsKeyword(c))
                        return new SqlToken(c.ToUpperInvariant(), SqlTokenType.Keyword);
                    return new SqlToken(c.ToLowerInvariant(), SqlTokenType.Identifier);
                });
            lexer.Replace("regularIdentifier", regularIdentifier);
            return lexer;
        }

        public SqlStatementListNode Parse(string s)
        {
            var lexer = _lexer.Value;
            var input = new StringCharacterSequence(s);
            var tokens = lexer.ToSequence(input).Select(r => r.Value);

            var parser = _parser.Value;
            var result = parser.Parse(tokens);
            var unparsedRemainder = input.GetRemainder();
            return result.Value as SqlStatementListNode;
        }
    }
}
