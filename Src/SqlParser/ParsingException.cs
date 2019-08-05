using System;
using System.Runtime.Serialization;
using SqlParser.Tokenizing;

namespace SqlParser
{
    [Serializable]
    public class ParsingException : Exception
    {
        public ParsingException()
        {
        }

        public ParsingException(string message) : base(message)
        {
        }

        public ParsingException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ParsingException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static ParsingException UnexpectedCharacter(char expected, char found, Location l)
        {
            return new ParsingException($"Expected {expected} but found {found} at {l}");
        }

        public static ParsingException UnexpectedCharacter(char found, Location l)
        {
            return new ParsingException($"Unexpected character {found} at {l}");
        }

        public static ParsingException UnexpectedToken(SqlTokenType type, SqlToken found)
        {
            return new ParsingException($"Expecting token with type={type} but found type={found.Type}, value={found.Value} at {found.Location}");
        }

        public static ParsingException UnexpectedToken(SqlTokenType type, string[] values, SqlToken found)
        {
            return new ParsingException($"Expecting token with type={type}, value={string.Join(",", values)} but found type={found.Type}, value={found.Value} at {found.Location}");
        }

        public static ParsingException CouldNotParseRule(string rule, SqlToken found)
        {
            return new ParsingException($"Could not parse {rule}, unexpected token {found} at {found.Location}");
        }
    }
}