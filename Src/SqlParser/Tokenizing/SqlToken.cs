using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using SqlParser.SqlStandard;

namespace SqlParser.Tokenizing
{
    public class SqlToken : IDiagnosable
    {
        public string Value { get; }
        public SqlTokenType Type { get; }
        public Location Location { get; set;  }

        public SqlToken(string s, SqlTokenType type)
        {
            Value = s;
            Type = type;
        }

        public SqlToken(string s, SqlTokenType type, Location location)
        {
            Value = s;
            Type = type;
            Location = location;
        }

        public static SqlToken EndOfInput() => new SqlToken(null, SqlTokenType.EndOfInput, null);

        public static SqlToken Variable(string name, Location location) => new SqlToken(name, SqlTokenType.Variable, location);

        public static SqlToken Number(string s, Location location) => new SqlToken(s, SqlTokenType.Number, location);

        public static SqlToken Word(string s, Location location)
        {
            if (Facts.IsKeyword(s))
                return new SqlToken(s.ToUpperInvariant(), SqlTokenType.Keyword, location);
            return new SqlToken(s, SqlTokenType.Identifier, location);
        }

        public static SqlToken Keyword(string s, Location location) => new SqlToken(s, SqlTokenType.Keyword, location);
 
        public static SqlToken Symbol(string c, Location location) => new SqlToken(c, SqlTokenType.Symbol, location);

        public static SqlToken Whitespace(string s, Location location) => new SqlToken(s, SqlTokenType.Whitespace, location);

        public static SqlToken QuotedString(string s, Location location) => new SqlToken(s, SqlTokenType.QuotedString, location);

        public static SqlToken QuotedIdentifier(string s, Location location) => new SqlToken(s, SqlTokenType.Identifier, location);

        public static SqlToken Comment(string s, Location location) => new SqlToken(s, SqlTokenType.Comment, location);

        public bool IsKeyword() => IsType(SqlTokenType.Keyword);

        public bool IsKeyword(params string[] keywords)
        {
            if (Type != SqlTokenType.Keyword)
                return false;
            foreach (var keyword in keywords)
            {
                if (Value == keyword)
                    return true;
            }

            return false;
        }

        public bool IsSymbol(params string[] symbols)
        {
            if (Type != SqlTokenType.Symbol)
                return false;
            foreach (var symbol in symbols)
            {
                if (Value == symbol)
                    return true;
            }

            return false;
        }

        public bool IsType(params SqlTokenType[] types) => types.Any(t => Type == t);

        public bool Is(SqlTokenType type, params string[] values) => IsType(type) && values.Any(v => Value == v);

        // Used primarily for debugging/testing purposes
        public override string ToString() => $"{Type}: '{Value}'";

        private List<ParseError> _errors;
        public IEnumerable<ParseError> Errors => _errors;
        public void AddErrors(IEnumerable<ParseError> errors)
        {
            if (_errors == null)
                _errors = new List<ParseError>();
            _errors.AddRange(errors);
        }
    }
}
