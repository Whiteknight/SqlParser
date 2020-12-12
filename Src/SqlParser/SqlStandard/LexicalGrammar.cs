using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using SqlParser.Tokenizing;
using static ParserObjects.ParserMethods;
using static ParserObjects.ParserMethods<char>;

namespace SqlParser.SqlStandard
{
    // https://ronsavage.github.io/SQL/sql-2003-2.bnf.html#character%20set%20specification
    public static class LexicalGrammar
    {
        public static IParser<char, SqlToken> CreateParser() => InitializeParsers();

        private static IParser<char, SqlToken> InitializeParsers()
        {
            var idStart = Match(c => char.IsLetter(c) || c == '_');
            var idPart = Match(c => char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '$' || c == '@' || c == '#');
            var regularIdentifier = Rule(
                    idStart,
                    idPart.List().Transform(c => c.ToArray()),

                    (start, parts) => start + new string(parts)
                )
                .Transform(c => new SqlToken(c, Facts.IsKeyword(c) ? SqlTokenType.Keyword : SqlTokenType.Identifier))
                .Replaceable()
                .Named("regularIdentifier");

            var variable = Rule(
                Match(c => c == '$' || c == '@'),
                idPart.List().Transform(c => new string(c.ToArray())),

                (prefix, name) => prefix + name
            );

            var delimitedIdentifierPart = First(
                Match(c => c != '"').Transform(c => c.ToString()),
                Match("\"\"").Transform(c => new string(c.ToArray()))
            );
            var delimitedIdentifier = Rule(
                    Match('\"'),
                    delimitedIdentifierPart.List().Transform(s => string.Join("", s)),
                    Match('\"'),

                    (open, parts, close) => parts
                )
                .Replaceable()
                .Named("delimitedIdentifier")
                .Transform(c => new SqlToken(c, SqlTokenType.Identifier));

            // TODO: Need this to be case-insensitive
            var booleanLiteral = First(
                CharacterString("TRUE"),
                CharacterString("FALSE"),
                CharacterString("UNKNOWN")
            );

            var sign = First(
                CharacterString("+"),
                CharacterString("-"),
                Produce(() => "")
            );
            var integer = Rule(
                sign,
                Match(char.IsDigit).ListCharToString(true),
                (s, value) => s + value
            );
            var number = Rule(
                integer,
                CharacterString("."),
                integer,
                (whole, dot, fract) => $"{whole}.{fract}"
            );

            var hexDigits = new HashSet<char>("0123456789abcdefABCDEF");
            var hexit = Match(c => hexDigits.Contains(c));

            var binaryStringLiteral = Rule(
                CharacterString("X\'"),
                hexit.ListCharToString(),
                CharacterString("\'"),
                (introducer, body, terminator) => introducer + body + terminator
            );
            var stringCharacter = First(
                CharacterString("\'\'").Transform(c => '\''),
                Match(c => c != '\'')
            );
            var nationalStringLiteral = Rule(
                CharacterString("N\'"),
                stringCharacter.List().Transform(s => string.Join("", s)),
                CharacterString("\'"),
                (introducer, body, terminator) => introducer + body + terminator
            );
            var stringLiteral = Rule(
                    CharacterString("\'"),
                    stringCharacter.List().Transform(s => new string(s.ToArray())),
                    CharacterString("\'"),
                    (introducer, body, terminator) =>
                        body
                )
                .Replaceable()
                .Named("stringLiteral")
                .Transform(c => new SqlToken(c, SqlTokenType.QuotedString));

            var identifier = First(
                // If it's not bracketed, it might be a keyword. If it is bracketed, it's
                // always an identifier
                regularIdentifier,
                delimitedIdentifier
            );

            var operators = Trie<string>(trie => trie
                    .Add("<>")
                    .Add(">=")
                    .Add("<=")
                    .Add("::")
                    .Add("..")
                    .Add("+=")
                    .Add("-=")
                    .Add("*=")
                    .Add("/=")
                    .Add("%=")
                    .Add("&=")
                    .Add("^=")
                    .Add("|=")
                    .Add("=")
                    .Add("%")
                    .Add("&")
                    .Add("(")
                    .Add(")")
                    .Add("*")
                    .Add("+")
                    .Add("-")
                    .Add(",")
                    .Add(".")
                    .Add(":")
                    .Add(";")
                    .Add("<")
                    .Add(">")
                    .Add("?")
                    .Add("^")
                    .Add("|")
                    .Add("~")
                )
                .Transform(s => new SqlToken(s, SqlTokenType.Symbol));

            var tokens = First(
                    If(End(), Produce(() => new SqlToken(null, SqlTokenType.EndOfInput))),
                    variable.Transform(c => new SqlToken(c, SqlTokenType.Variable)),
                    // TODO: Unquoted identifiers might be keywords
                    identifier,
                    stringLiteral,
                    //nationalStringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                    // TODO: Binary literal isn't really a quoted string
                    //binaryStringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                    // TODO: Unicode strings,
                    // TODO: Interval literals
                    // TODO: Date/Time literals
                    //booleanLiteral.Transform(c => new SqlToken(c, SqlTokenType.Keyword)),
                    number.Transform(c => new SqlToken(c, SqlTokenType.Number)),
                    integer.Transform(c => new SqlToken(c, SqlTokenType.Number)),
                    operators,
                    Produce((s, d) => new SqlToken(s.GetNext().ToString(), SqlTokenType.Unknown))
                )
            ;

            var simpleComment = Rule(
                CharacterString("--"),
                Match(c => c != '\r' && c != '\n').List(),

                (introducer, commentChars) => ""
            );
            var bracketedCommentContentChar = First(
                Rule(
                    CharacterString("*"),
                    Match(c => c != '/'),

                    (asterisk, slash) => new[] { asterisk[0], slash }
                ),
                Match(c => c != '*').Transform(c => new[] { c })
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
            var whitespace = Whitespace();
            var separatorItem = First(
                comment,
                whitespace
            );
            var separator = separatorItem.List();
            return Rule(
                separator,
                tokens,
                (s, t) => t
           ).Examine(
                s =>
                { },
                s =>
                { }
            );
        }
    }
}
