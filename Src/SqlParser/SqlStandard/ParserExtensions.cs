using System.Security.Cryptography.X509Certificates;
using ParserObjects;
using ParserObjects.Parsers;
using SqlParser.Ast;
using SqlParser.Tokenizing;
using static ParserObjects.Parsers.ParserMethods;
using static SqlParser.SqlStandard.ParserMethods;

namespace SqlParser.SqlStandard
{
    public static class ParserExtensions
    {
        // TODO: Move these into the grammar
        private static readonly IParser<SqlToken, SqlToken> _openParen = Token(SqlTokenType.Symbol, "(");
        private static readonly IParser<SqlToken, SqlToken> _closeParen = Token(SqlTokenType.Symbol, ")");

        public static IParser<SqlToken, SqlParenthesisNode<TNode>> Parenthesized<TNode>(this IParser<SqlToken, TNode> parser) 
            where TNode : class, ISqlNode
        {
            return Rule(
                _openParen,
                parser,
                _closeParen,
                (o, value, c) => new SqlParenthesisNode<TNode>(value)
            );
        }

        public static IParser<SqlToken, SqlParenthesisNode<TNode>> MaybeParenthesized<TNode>(this IParser<SqlToken, TNode> parser)
            where TNode : class, ISqlNode
        {
            return First(
                Rule(
                    _openParen,
                    parser,
                    _closeParen,
                    (o, value, c) => new SqlParenthesisNode<TNode>(value)
                ),
                parser.Transform(x => new SqlParenthesisNode<TNode>(x))
            );
        }
    }
}
