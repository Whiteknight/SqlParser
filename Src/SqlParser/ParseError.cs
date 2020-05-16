using ParserObjects;

namespace SqlParser
{
    public class ParseError
    {
        public ParseError(Location location, string errorMessage)
        {
            Location = location;
            ErrorMessage = errorMessage;
        }

        public Location Location { get; }
        public string ErrorMessage { get; }
    }
}