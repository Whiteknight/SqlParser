using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using SqlParser.Tokenizing;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;

namespace SqlParser.SqlStandard
{
    // https://ronsavage.github.io/SQL/sql-2003-2.bnf.html#character%20set%20specification
    public static partial class LexicalGrammar
    {
        private static readonly Lazy<IParser<char, SqlToken>> _parser = new Lazy<IParser<char, SqlToken>>(InitializeParsers);

        public static IParser<char, SqlToken> GetParser() => _parser.Value;

        private static IParser<char, SqlToken> InitializeParsers()
        {
            var idStart = Match<char>(c => char.IsLetter(c) || c == '_');
            var idPart = Match<char>(c => char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '$' || c == '@');
            var regularIdentifier = Rule(
                idStart,
                idPart.List().Transform(c => c.ToArray()),

                (start, parts) => start + new string(parts)
            );
            var variable = Rule(
                Match<char>(c => c == '$' || c == '@'),
                regularIdentifier,

                (prefix, name) => prefix + name
            );
            var delimitedIdentifierPart = First(
                Match<char>(c => c != '"').Transform(c => c.ToString()),
                Match<char>("\"\"").Transform(c => new string(c.ToArray()))
            );
            var delimitedIdentifier = Rule(
                Match('\"'),
                delimitedIdentifierPart.List().Transform(s => string.Join("", s)),
                Match('\"'),

                (open, parts, close) => parts
            );
            var notEqualsOperator = CharacterString("<>");
            var greaterThanOrEqualsOperator = CharacterString(">=");
            var lessThanOrEqualsOperator = CharacterString("<=");
            var concatenationOperator = CharacterString("||");
            var rightArrow = CharacterString("->");
            var doubleColon = CharacterString("::");
            var doublePeriod = CharacterString("..");
            var simpleComment = Rule(
                CharacterString("--"),
                Match<char>(c => c != '\r' && c != '\n').List(),
                Optional(Match<char>(c => c == '\r'), () => '\0'),
                Match<char>(c => c == '\n'),

                (introducer, commentChars, carriageReturn, newline) => ""
            );
            var bracketedCommentContentChar = First(
                Rule(
                    CharacterString("*"),
                    Match<char>(c => c != '/'),

                    (asterisk, slash) => new[] { asterisk[0], slash }
                ),
                Match<char>(c => c != '*').Transform(c => new[] { c })
            );
            var bracketedComment = Rule(
                CharacterString("/*"),
                bracketedCommentContentChar.List(),
                CharacterString("*/"),

                (introducer, body, terminator) => ""
            );
            var comment = First(
                simpleComment,
                bracketedComment
            );
            var whitespace = Match<char>(char.IsWhiteSpace).List();
            var separatorItem = First(
                comment,
                whitespace
            );
            var separator = separatorItem.List();

            // TODO: Need this to be case-insensitive
            var booleanLiteral = First(
                CharacterString("TRUE"),
                CharacterString("FALSE"),
                CharacterString("UNKNOWN")
            );

            var sign = First(
                CharacterString("+"),
                CharacterString("-")
            );
            var unsignedInteger = Match<char>(char.IsDigit).ListCharToString();
            var signedInteger = Rule(
                sign,
                Match<char>(char.IsDigit).ListCharToString(),
                (s, value) => s + value
            );

            var hexDigits = new HashSet<char>("0123456789abcdefABCDEF");
            var hexit = Match<char>(c => hexDigits.Contains(c));
            var binaryStringLiteral = Rule(
                CharacterString("X\""),
                hexit.ListCharToString(),
                CharacterString("\""),
                (introducer, body, terminator) => introducer + body + terminator
            );
            var stringCharacter = First(
                CharacterString("\"\""),
                Match<char>(c => c != '"').Transform(c => c.ToString())
            );
            var nationalStringLiteral = Rule(
                CharacterString("N\""),
                stringCharacter.List().Transform(s => string.Join("", s)),
                CharacterString("\""),
                (introducer, body, terminator) => introducer + body + terminator
            );
            var stringLiteral = Rule(
                CharacterString("\""),
                stringCharacter.List().Transform(s => string.Join("", s)),
                CharacterString("\""),
                (introducer, body, terminator) => introducer + body + terminator
            );

            var identifier = First(
                regularIdentifier,
                delimitedIdentifier
            );

            var tokens = First(
                variable.Transform(c => new SqlToken(c, SqlTokenType.Variable)),
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
                CharacterString("%").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("&").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("(").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString(")").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("*").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("+").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString(",").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("-").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString(".").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("/").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString(":").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString(";").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("<").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("=").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString(">").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("?").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("[").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("]").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("^").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("|").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("{").Transform(c => new SqlToken(c, SqlTokenType.Symbol)),
                CharacterString("}").Transform(c => new SqlToken(c, SqlTokenType.Symbol))
            );

            return tokens;
        }
    }
}
