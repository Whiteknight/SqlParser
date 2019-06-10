using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public interface ISqlSymbolScopeNode
    {
        SymbolTable Symbols { get; set; }
    }
}