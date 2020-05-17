using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using ParserObjects.Sequences;
using SqlParser.Ast;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.ParserMethods<char>;

namespace SqlParser.SqlServer.Parsing
{
    public class Parser 
    {
        public SqlStatementListNode Parse(string sql)
        {
            var lexer = SqlStandard.LexicalGrammar.GetParser();
            var stringCharacter = First(
                CharacterString("\'\'").Transform(c => '\''),
                Match(c => c != '\'')
            );
            var stringLiteral = Rule(
                CharacterString("\'"),
                stringCharacter.List().Transform(s => new string(s.ToArray())),
                CharacterString("\'"),
                (introducer, body, terminator) =>
                    body
            );
            bool ok = lexer.Replace("stringLiteral", stringLiteral);
            var input = new StringCharacterSequence(sql);
            var tokens = lexer.ToSequence(input).Select(r => r
                .Value);

            var parser = SqlStandard.SqlStandardGrammar.GetParser();
            var result = parser.Parse(tokens);
            var unparsedRemainder = input.GetRemainder();
            return result.Value as SqlStatementListNode;
        }
    }
}
