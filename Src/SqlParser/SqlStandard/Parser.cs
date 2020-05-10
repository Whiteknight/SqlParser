using ParserObjects;
using ParserObjects.Sequences;
using SqlParser.Ast;

namespace SqlParser.SqlStandard
{
    public class Parser
    {
        public SqlStatementListNode Parse(string s)
        {
            var lexer = SqlStandard.LexicalGrammar.GetParser();
            var input = new StringCharacterSequence(s);
            var tokens = lexer.ToSequence(input).Select(r => r
                .Value);

            var parser = SqlStandard.SqlStandardGrammar.GetParser();
            var result = parser.Parse(tokens);
            var unparsedRemainder = input.GetRemainder();
            return result.Value as SqlStatementListNode;
        }
    }
}
