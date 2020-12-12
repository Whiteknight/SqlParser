using ParserObjects;
using SqlParser.Ast;
using SqlParser.SqlStandard;
using SqlParser.Tokenizing;
using static ParserObjects.ParserMethods<SqlParser.Tokenizing.SqlToken>;

namespace SqlParser.Parsing
{
    public static class ParserExtensions
    {
        public static IParser<SqlToken, SqlParenthesisNode<TNode>> Parenthesized<TNode>(this IParser<SqlToken, TNode> parser)
            where TNode : class, ISqlNode
        {
            return Rule(
                SqlStandardGrammar.OpenParen,
                parser,
                SqlStandardGrammar.CloseParen,
                (o, value, c) => new SqlParenthesisNode<TNode>(value)
            );
        }

        public static IParser<SqlToken, TNode> MaybeParenthesized<TNode>(this IParser<SqlToken, TNode> parser)
            where TNode : class, ISqlNode
        {
            return First(
                Rule(
                    SqlStandardGrammar.OpenParen,
                    parser,
                    SqlStandardGrammar.CloseParen,
                    (o, value, c) => value
                ),
                parser
            );
        }
    }
}
