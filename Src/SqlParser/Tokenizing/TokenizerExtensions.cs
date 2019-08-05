namespace SqlParser.Tokenizing
{
    public static class TokenizerExtensions
    {
        public static SqlToken GetNext(this ITokenizer tokenizer, bool skipComments = true)
        {
            while (true)
            {
                var next = tokenizer.GetNextToken();
                if (next == null || next.IsType(SqlTokenType.EndOfInput))
                    return SqlToken.EndOfInput();
                if (next.Type == SqlTokenType.Comment && skipComments)
                    continue;
                if (next.Type == SqlTokenType.Whitespace)
                    continue;
                return next;
            }
        }

        public static bool NextIs(this ITokenizer tokenizer, SqlTokenType type, string value, bool consume = false)
        {
            var t = tokenizer.GetNext();
            bool isSame = t.Type == type && t.Value == value;
            if (!isSame)
            {
                tokenizer.PutBack(t);
                return false;
            }

            if (!consume)
                tokenizer.PutBack(t);
            return true;
        }

        public static SqlToken Expect(this ITokenizer tokenizer, SqlTokenType type)
        {
            var found = tokenizer.GetNext();
            if (found.Type != type)
                throw ParsingException.UnexpectedToken(type, found);
            return found;
        }

        public static SqlToken Expect(this ITokenizer tokenizer, SqlTokenType type, params string[] values)
        {
            var found = tokenizer.GetNext();
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

        public static SqlToken Peek(this ITokenizer tokenizer)
        {
            var t = tokenizer.GetNext();
            tokenizer.PutBack(t);
            return t;
        }

        public static SqlToken ExpectPeek(this ITokenizer tokenizer, SqlTokenType type)
        {
            var found = tokenizer.Peek();
            if (found.Type != type)
                throw ParsingException.UnexpectedToken(type, found);
            return found;
        }

        public static void Skip(this ITokenizer tokenizer, SqlTokenType type)
        {
            while (true)
            {
                var t = tokenizer.GetNext();
                if (t.Type == SqlTokenType.EndOfInput || type == SqlTokenType.EndOfInput)
                    break;
                if (t.Type != type)
                {
                    tokenizer.PutBack(t);
                    break;
                }
            }
        }
    }
}