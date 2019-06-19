using SqlParser.Symbols;

namespace SqlParser.Ast
{
    public interface ISqlSymbolScopeNode
    {
        SymbolTable Symbols { get; set; }
    }
}