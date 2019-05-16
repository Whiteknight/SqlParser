namespace CastIron.SqlParsing.Tokenizing
{
    public class SqlToken
    {
        public string Value { get; }
        public SqlTokenType Type { get; }
        public Location Location { get; }

        public SqlToken(string s, SqlTokenType type, Location location)
        {
            Value = s;
            Type = type;
            Location = location;
        }

        public static SqlToken EndOfInput()
        {
            return new SqlToken(null, SqlTokenType.EndOfInput, null);
        }

        public static SqlToken Variable(string name, Location location)
        {
            return new SqlToken(name, SqlTokenType.Variable, location);
        }

        public static SqlToken Number(string s, Location location)
        {
            return new SqlToken(s, SqlTokenType.Number, location);
        }

        public static SqlToken Word(string s, Location location)
        {
            if (Keywords.IsKeyword(s))
                return new SqlToken(s.ToUpperInvariant(), SqlTokenType.Keyword, location);
            return new SqlToken(s, SqlTokenType.Identifier, location);
        }

        public static SqlToken Symbol(string c, Location location)
        {
            return new SqlToken(c, SqlTokenType.Symbol, location);
        }

        public static SqlToken Whitespace(string s, Location location)
        {
            return new SqlToken(s, SqlTokenType.Whitespace, location);
        }

        public static SqlToken QuotedString(string s, Location location)
        {
            return new SqlToken(s, SqlTokenType.QuotedString, location);
        }

        public static SqlToken BracketedIdentifier(string s, Location location)
        {
            return new SqlToken(s, SqlTokenType.Identifier, location);
        }

        public static SqlToken Comment(string s, Location location)
        {
            return new SqlToken(s, SqlTokenType.Comment, location);
        }

        public bool IsKeyword()
        {
            return IsType(SqlTokenType.Keyword);
        }

        public bool IsKeyword(string keyword)
        {
            return Type == SqlTokenType.Keyword && Value == keyword;
        }

        public bool IsType(SqlTokenType type)
        {
            return Type == type;
        }

        public bool Is(SqlTokenType type, string value)
        {
            return IsType(type) && Value == value;
        }

        // Used primarily for debugging/testing purposes
        public override string ToString()
        {
            return $"{Type}: '{Value}'";
        }
    }
}
