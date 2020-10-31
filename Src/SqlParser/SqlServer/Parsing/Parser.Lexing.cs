using ParserObjects;
using ParserObjects.Parsers;
using SqlParser.Tokenizing;
using static ParserObjects.Parsers.ParserMethods<char>;


namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
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
    }
}
