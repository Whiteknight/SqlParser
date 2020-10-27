namespace SqlParser.Tokenizing
{
    public enum SqlTokenType
    {
        Unknown,
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