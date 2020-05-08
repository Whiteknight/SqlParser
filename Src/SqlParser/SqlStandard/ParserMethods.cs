using System.Linq;
using ParserObjects;
using SqlParser.Ast;
using SqlParser.Tokenizing;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;

namespace SqlParser.SqlStandard
{
    public static class ParserMethods
    {
        public static IParser<SqlToken, SqlKeywordNode> Keyword(params string[] words)
        {
            var matchers = words.Select(w => Match<SqlToken>(t => t.IsKeyword(w))).ToList();
            if (matchers.Count == 1)
                return matchers.First().Transform(t => new SqlKeywordNode(t));
            // TODO: Clean this up
            return new RuleParser<SqlToken, SqlKeywordNode>(matchers, r =>
            {
                var tokens = r.Cast<SqlToken>().ToList();
                var keyword = string.Join(" ", tokens.Select(t => t.Value));
                var location = tokens.First().Location;
                return new SqlKeywordNode(keyword, location);
            });
        }

        public static IParser<SqlToken, SqlOperatorNode> Operator(string op)
        {
            return Match<SqlToken>(t => t.IsSymbol(op)).Transform(t => new SqlOperatorNode(t));
        }

        // TODO: Move these into the grammar
        private static readonly IParser<SqlToken, SqlToken> _openParen = Token(SqlTokenType.Symbol, "(");
        private static readonly IParser<SqlToken, SqlToken> _closeParen = Token(SqlTokenType.Symbol, ")");

        public static IParser<SqlToken, SqlParenthesisNode<TNode>> Parenthesized<TNode>(IParser<SqlToken, TNode> parser)
            where TNode : class, ISqlNode
        {
            return Rule(
                _openParen,
                parser,
                _closeParen,
                (o, value, c) => new SqlParenthesisNode<TNode>(value)
            );
        }

        public static IParser<SqlToken, SqlToken> Token(SqlTokenType type)
        {
            return Match<SqlToken>(t => t.IsType(type));
        }

        public static IParser<SqlToken, SqlToken> Token(SqlTokenType type, string value)
        {
            return Match<SqlToken>(t => t.IsType(type) && t.Value == value);
        }

    }

}
