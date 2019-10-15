namespace SqlParser.Symbols
{
    // Determines where the thing came from
    public enum SymbolOriginKind
    {
        Unknown,
        Alias,
        UserDeclared,
        Environmental
    }
}