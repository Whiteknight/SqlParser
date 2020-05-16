namespace SqlParser.Tokenizing
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