using SqlParser.Symbols;

namespace SqlParser
{
    public interface ISqlSymbolScopeNode
    {
        SymbolTable Symbols { get; set; }
    }
}