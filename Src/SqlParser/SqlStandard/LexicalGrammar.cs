using System.Collections.Generic;
using System.Linq;
using SqlParser.Parsing.Parsers;
using SqlParser.Tokenizing;
using static SqlParser.Parsing.Parsers.ParserMethods;

namespace SqlParser.SqlStandard
{
    // https://ronsavage.github.io/SQL/sql-2003-2.bnf.html#character%20set%20specification
    public class LexicalGrammar
    {
        public LexicalGrammar()
        {
            InitializeParsers();
        }

        private void InitializeParsers()
        {
            var idStart = Match<char>(c => char.IsLetter(c) || c == '_' || c == '$' || c == '@');
            var idPart = Match<char>(c => char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '$' || c == '@');
            var regularIdentifier = Rule(
                idStart,
                idPart.List(c => c.ToArray()),

                (start, parts) => start + new string(parts)
            );
            var delimitedIdentifierPart = First(
                Match<char, string>(c => c != '"', c => c.ToString()),
                Characters("\"\"", c => new string(c))
            );
            var delimitedIdentifier = Rule(
                Characters("\""),
                delimitedIdentifierPart.List(s => string.Join("", s)),
                Characters("\""),

                (open, parts, close) => parts
            );
            var notEqualsOperator = Characters("<>");
            var greaterThanOrEqualsOperator = Characters(">=");
            var lessThanOrEqualsOperator = Characters("<=");
            var concatenationOperator = Characters("||");
            var rightArrow = Characters("->");
            var doubleColon = Characters("::");
            var doublePeriod = Characters("..");
            var simpleComment = Rule(
                Characters("--"),
                Match<char>(c => c != '\r' && c != '\n').List(c => c),
                Optional(Match<char>(c => c == '\r'), () => '\0'),
                Match<char>(c => c == '\n'),

                (introducer, commentChars, carriageReturn, newline) => ""
            );
            var bracketedCommentContentChar = First(
                Rule(
                    Characters("*", c => '*'),
                    Match<char>(c => c != '/'),

                    (asterisk, slash) => new[] { asterisk, slash }
                ),
                Match<char, char[]>(c => c != '*', c => new[] { c })
            );
            var bracketedComment = Rule(
                Characters("/*"),
                bracketedCommentContentChar.List(c => ""),
                Characters("*/"),

                (introducer, body, terminator) => ""
            );
            var comment = First(
                simpleComment,
                bracketedComment
            );
            var whitespace = Match<char>(char.IsWhiteSpace).List(c => "");
            var separatorItem = First(
                    comment,
                    whitespace
                );
            var separator = separatorItem.List(c => "");

            // TODO: Need this to be case-insensitive
            var booleanLiteral = First(
                Characters("TRUE"),
                Characters("FALSE"),
                Characters("UNKNOWN")
            );

            var sign = First(
                Characters("+"),
                Characters("-")
            );
            var unsignedInteger = Match<char>(char.IsDigit).List(c => new string(c.ToArray()));
            var signedInteger = Rule(
                sign,
                Match<char>(char.IsDigit).List(c => new string(c.ToArray())),
                (s, value) => s + value
            );

            var hexDigits = new HashSet<char>("0123456789abcdefABCDEF");
            var hexit = Match<char>(c => hexDigits.Contains(c));
            var binaryStringLiteral = Rule(
                Characters("X\""),
                hexit.List(c => new string(c.ToArray())),
                Characters("\""),
                (introducer, body, terminator) => introducer + body + terminator
            );
            var stringCharacter = First(
                Characters("\"\""),
                Match<char, string>(c => c != '"', c => c.ToString())
            );
            var nationalStringLiteral = Rule(
                Characters("N\""),
                stringCharacter.List(s => string.Join("", s)),
                Characters("\""),
                (introducer, body, terminator) => introducer + body + terminator
            );
            var stringLiteral = Rule(
                Characters("\""),
                stringCharacter.List(s => string.Join("", s)),
                Characters("\""),
                (introducer, body, terminator) => introducer + body + terminator
            );

            var identifier = First(
                regularIdentifier,
                delimitedIdentifier
            );

            var tokens = First(
                // TODO: Unquoted identifiers might be keywords
                identifier.Transform(c => new SqlToken(c, SqlTokenType.Identifier)),
                stringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                nationalStringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                // TODO: Binary literal isn't really a quoted string
                binaryStringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                // TODO: Unicode strings,
                // TODO: Interval literals
                // TODO: Date/Time literals
                booleanLiteral.Transform(c => new SqlToken(c, SqlTokenType.Keyword)),
                signedInteger.Transform(c => new SqlToken(c, SqlTokenType.Number)),
                unsignedInteger.Transform(c => new SqlToken(c, SqlTokenType.Number)),
                notEqualsOperator.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                greaterThanOrEqualsOperator.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                lessThanOrEqualsOperator.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                concatenationOperator.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                rightArrow.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                doubleColon.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                doublePeriod.Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("%").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("&").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("(").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters(")").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("*").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("+").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters(",").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("-").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters(".").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("/").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters(":").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters(";").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("<").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("=").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters(">").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("?").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("[").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("]").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("^").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("|").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("{").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                Characters("}").Transform(c => new SqlToken(c, SqlTokenType.Symbol))
            );
        }
    }
}
