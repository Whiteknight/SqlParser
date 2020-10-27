using System;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;
using SqlParser.Ast;
using SqlParser.Tokenizing;
using static ParserObjects.Parsers.ParserMethods<char>;

namespace SqlParser.SqlServer.Parsing
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

            //var stringCharacter = First(
            //    CharacterString("\'\'").Transform(c => '\''),
            //    Match(c => c != '\'')
            //);
            //var stringLiteral = Rule(
            //    CharacterString("\'"),
            //    stringCharacter.List().Transform(s => new string(s.ToArray())),
            //    CharacterString("\'"),
            //    (introducer, body, terminator) =>
            //        body
            //);
            //var ok = lexer.Replace("stringLiteral", stringLiteral);

            var delimitedIdentifierPart = Match(c => c != ']');
            var delimitedIdentifier = Rule(
                Match('['),
                delimitedIdentifierPart.List().Transform(s => string.Join("", s)),
                Match(']'),

                (open, parts, close) => parts
            );
            var ok = lexer.Replace("delimitedIdentifier", delimitedIdentifier);

            return lexer;
        }

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
