using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using SqlParser.Ast;
using SqlParser.SqlStandard;
using SqlParser.Tokenizing;
using static ParserObjects.ParserMethods<SqlParser.Tokenizing.SqlToken>;

namespace SqlParser.Parsing
{
    public static class ParserMethods
    {
        public static IParser<SqlToken, SqlKeywordNode> Keyword(params string[] words)
        {
            var matchers = words.Select(w => Match(t => t
                .IsKeyword(w))).ToList();
            if (matchers.Count == 1)
                return matchers.First().Transform(t =>
                    new SqlKeywordNode(t));
            // TODO: Clean this up
            return new RuleParser<SqlToken, SqlKeywordNode>(matchers, r =>
            {
                var tokens = r.Cast<SqlToken>().ToList();
                var keyword = string.Join(" ", tokens.Select(t => t.Value));
                var location = tokens.First().Location;
                return new SqlKeywordNode(keyword, location);
            });
        }

        public static IParser<SqlToken, SqlKeywordNode> RequiredKeyword(params string[] words)
        {
            return First(
                Keyword(words),
                Produce((i, d) =>
                {
                    var word = string.Join(" ", words);
                    var n = new SqlKeywordNode(word, i.CurrentLocation);
                    n.AddErrors(i.CurrentLocation, $"Missing required keyword '{word}'");
                    return n;
                })
            );
        }

        public static IParser<SqlToken, SqlOperatorNode> Operator(params string[] op)
        {
            return Match(t => t.IsSymbol(op))
                .Transform(t => new SqlOperatorNode(t));
        }

        public static IParser<SqlToken, SqlOperatorNode> RequiredOperator(params string[] op)
        {
            return First(
                Operator(op),
                ErrorNode<SqlOperatorNode>($"Expecing operator {op}")
            );
        }

        public static IParser<SqlToken, TNode> ErrorNode<TNode>(string error)
            where TNode : IDiagnosable, new()
        {
            return Produce((i, d) =>
            {
                var t = new TNode();
                t.AddErrors(i.CurrentLocation, error);
                return t;
            });
        }

        public static IParser<SqlToken, SqlParenthesisNode<TNode>> Parenthesized<TNode>(IParser<SqlToken, TNode> parser)
            where TNode : class, ISqlNode
        {
            return Rule(
                SqlStandardGrammar.OpenParen,
                parser,
                SqlStandardGrammar.CloseParen,
                (o, value, c) => new SqlParenthesisNode<TNode>(value)
            );
        }

        public static IParser<SqlToken, SqlToken> Token(SqlTokenType type)
        {
            return Match(t => t.IsType(type));
        }

        public static IParser<SqlToken, SqlToken> Token(SqlTokenType type, string value)
        {
            return Match(t => t.IsType(type) && t.Value == value);
        }

        public static IParser<SqlToken, SqlToken> RequiredToken(SqlTokenType type, string value)
        {
            return First(
                Token(type, value),
                Produce((i, d) =>
                {
                    var t = new SqlToken(value, type, i.CurrentLocation);
                    t.AddErrors(i.CurrentLocation, $"Missing {type} '{value}'");
                    return t;
                })
            );
        }
    }
}
