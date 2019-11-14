using System.Collections.Generic;
using System.Linq;

namespace SqlParser.Tokenizing
{
    public static class ITokenizerExtensions
    {
        public static SqlToken GetNext(this ITokenizer ITokenizer, bool skipComments = true)
        {
            while (true)
            {
                var next = ITokenizer.ScanNext();
                if (next == null || next.IsType(SqlTokenType.EndOfInput))
                    return SqlToken.EndOfInput();
                if (next.Type == SqlTokenType.Comment && skipComments)
                    continue;
                if (next.Type == SqlTokenType.Whitespace)
                    continue;
                return next;
            }
        }

        public static bool NextIs(this ITokenizer ITokenizer, SqlTokenType type, string value, bool consume = false)
        {
            var t = ITokenizer.GetNext();
            bool isSame = t.Type == type && t.Value == value;
            if (!isSame)
            {
                ITokenizer.PutBack(t);
                return false;
            }

            if (!consume)
                ITokenizer.PutBack(t);
            return true;
        }

        public static SqlToken Expect(this ITokenizer ITokenizer, SqlTokenType type)
        {
            var found = ITokenizer.GetNext();
            if (found.Type != type)
                throw ParsingException.UnexpectedToken(type, found);
            return found;
        }

        public static SqlToken Expect(this ITokenizer ITokenizer, SqlTokenType type, params string[] values)
        {
            var found = ITokenizer.GetNext();
            if (found.Type == type)
            {
                foreach (var value in values)
                {
                    if (found.Value == value)
                        return found;
                }
            }

            throw ParsingException.UnexpectedToken(type, values, found);
        }

        public static SqlToken Peek(this ITokenizer ITokenizer)
        {
            var t = ITokenizer.GetNext();
            ITokenizer.PutBack(t);
            return t;
        }

        public static SqlToken ExpectPeek(this ITokenizer ITokenizer, SqlTokenType type)
        {
            var found = ITokenizer.Peek();
            if (found.Type != type)
                throw ParsingException.UnexpectedToken(type, found);
            return found;
        }

        public static void Skip(this ITokenizer ITokenizer, SqlTokenType type)
        {
            while (true)
            {
                var t = ITokenizer.GetNext();
                if (t.Type == SqlTokenType.EndOfInput || type == SqlTokenType.EndOfInput)
                    break;
                if (t.Type != type)
                {
                    ITokenizer.PutBack(t);
                    break;
                }
            }
        }

        public static SqlToken MaybeGetKeywordSequence(this ITokenizer ITokenizer, params string[] allowed)
        {
            var lookup = new HashSet<string>(allowed);
            var keywords = new List<SqlToken>();
            while (true)
            {
                var next = ITokenizer.GetNext();
                if (!next.IsType(SqlTokenType.Keyword))
                {
                    ITokenizer.PutBack(next);
                    break;
                }
                if (!lookup.Contains(next.Value))
                {
                    ITokenizer.PutBack(next);
                    break;
                }
                keywords.Add(next);
            }

            if (!keywords.Any())
                return null;

            var combined = string.Join(" ", keywords.Select(k => k.Value));
            return SqlToken.Keyword(combined, keywords.First().Location);
        }

        public static SqlToken GetIdentifierOrKeyword(this ITokenizer ITokenizer)
        {
            var next = ITokenizer.GetNext();
            if (next.IsType(SqlTokenType.Identifier) || next.IsType(SqlTokenType.Keyword))
                return next;
            throw ParsingException.UnexpectedToken(SqlTokenType.Identifier, next);
        }
    }
}