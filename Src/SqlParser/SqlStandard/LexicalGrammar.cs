using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ParserObjects;
using SqlParser.Tokenizing;
using ParserObjects.Parsers;
using static ParserObjects.Parsers.ParserMethods;
using static ParserObjects.Parsers.ParserMethods<char>;

namespace SqlParser.SqlStandard
{
    // https://ronsavage.github.io/SQL/sql-2003-2.bnf.html#character%20set%20specification
    public static class LexicalGrammar
    {
        private static readonly Lazy<IParser<char, SqlToken>> _parser = new Lazy<IParser<char, SqlToken>>(InitializeParsers);

        public static IParser<char, SqlToken> GetParser() => _parser.Value;

        private static IParser<char, SqlToken> InitializeParsers()
        {
            var idStart = Match(c => char.IsLetter(c) || c == '_');
            var idPart = Match(c => char.IsLetter(c) || char.IsDigit(c) || c == '_' || c == '$' || c == '@' || c == '#');
            var regularIdentifier = Rule(
                idStart,
                idPart.List().Transform(c => c.ToArray()),

                (start, parts) => start + new string(parts)
            );
            var variable = Rule(
                Match(c => c == '$' || c == '@'),
                idPart.List().Transform(c => new string(c.ToArray())),

                (prefix, name) => prefix + name
            );
            //var delimitedIdentifierPart = First(
            //    Match(c => c != '"').Transform(c => c.ToString()),
            //    Match("\"\"").Transform(c => new string(c.ToArray()))
            //);
            var delimitedIdentifierPart = First(
                Match(c => c != ']').Transform(c => c.ToString()),
                Match("[]").Transform(c => new string(c.ToArray()))
            );
            //var delimitedIdentifier = Rule(
            //    Match('\"'),
            //    delimitedIdentifierPart.List().Transform(s => string.Join("", s)),
            //    Match('\"'),

            //    (open, parts, close) => parts
            //);
            var delimitedIdentifier = Rule(
                Match('['),
                delimitedIdentifierPart.List().Transform(s => string.Join("", s)),
                Match(']'),

                (open, parts, close) => parts
            );

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
            var integer = Rule(
                sign.Optional(),
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
            //var binaryStringLiteral = Rule(
            //    CharacterString("X\""),
            //    hexit.ListCharToString(),
            //    CharacterString("\""),
            //    (introducer, body, terminator) => introducer + body + terminator
            //);
            //var stringCharacter = First(
            //    CharacterString("\"\""),
            //    Match(c => c != '"').Transform(c => c.ToString())
            //);
            //var nationalStringLiteral = Rule(
            //    CharacterString("N\""),
            //    stringCharacter.List().Transform(s => string.Join("", s)),
            //    CharacterString("\""),
            //    (introducer, body, terminator) => introducer + body + terminator
            //);
            //var stringLiteral = Rule(
            //    CharacterString("\""),
            //    stringCharacter.List().Transform(s => string.Join("", s)),
            //    CharacterString("\""),
            //    (introducer, body, terminator) => introducer + body + terminator
            //);

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
            );

            var identifier = First(
                // If it's not bracketed, it might be a keyword. If it is bracketed, it's
                // always an identifier
                regularIdentifier.Transform(c => new SqlToken(c, Facts.IsKeyword(c) ? SqlTokenType.Keyword : SqlTokenType.Identifier)),
                delimitedIdentifier.Transform(c => new SqlToken(c, SqlTokenType.Identifier))
            );

            var tokens = First(
                End().Transform(c => new SqlToken(null, SqlTokenType.EndOfInput)),
                variable.Transform(c => new SqlToken(c, SqlTokenType.Variable)),
                // TODO: Unquoted identifiers might be keywords
                identifier,
                stringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                //nationalStringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                // TODO: Binary literal isn't really a quoted string
                //binaryStringLiteral.Transform(c => new SqlToken(c, SqlTokenType.QuotedString)),
                // TODO: Unicode strings,
                // TODO: Interval literals
                // TODO: Date/Time literals
                //booleanLiteral.Transform(c => new SqlToken(c, SqlTokenType.Keyword)),
                number.Transform(c => new SqlToken(c, SqlTokenType.Number)),
                integer.Transform(c => new SqlToken(c, SqlTokenType.Number)),

                Trie<string>(trie => trie
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
                    .Transform(s => new SqlToken(s, SqlTokenType.Symbol))
            )
                .Examine(after: (p, i, r) => 
                    Debug.WriteLine($"Creating token {r.Value.Type}={r.Value.Value}"));

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
            var whitespace = Match(char.IsWhiteSpace).List(true);
            var separatorItem = First(
                comment,
                whitespace
            );
            var separator = separatorItem.List();
            return Rule(
                separator,
                tokens,
                (s, t) => t
            );
        }
    }
}
