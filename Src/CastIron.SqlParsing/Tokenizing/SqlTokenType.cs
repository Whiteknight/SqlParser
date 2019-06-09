namespace CastIron.SqlParsing.Tokenizing
{
    public enum SqlTokenType
    {
        EndOfInput,
        Number,
        Keyword,
        Symbol,
        Whitespace,
        QuotedString,
        Identifier,
        Variable,
        Comment
    }
}