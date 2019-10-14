using SqlParser.SqlServer.Symbols;

namespace SqlParser.Ast
{
    public interface ISqlSymbolScopeNode
    {
        SymbolTable Symbols { get; set; }
    }
}