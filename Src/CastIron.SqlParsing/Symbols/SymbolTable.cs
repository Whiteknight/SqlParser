using System.Collections.Generic;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Symbols
{
    public class SymbolTable
    {
        private readonly SymbolTable _parent;
        private readonly Dictionary<string, SymbolInfo> _symbols;

        public SymbolTable()
        {
            _symbols = new Dictionary<string, SymbolInfo>();
        }

        public SymbolTable(SymbolTable parent)
        {
            _parent = parent;
            _symbols = new Dictionary<string, SymbolInfo>();
        }

        public SymbolInfo GetInfo(string symbol)
        {
            if (_symbols.ContainsKey(symbol))
                return _symbols[symbol];
            return _parent?.GetInfo(symbol);
        }

        public SymbolInfo GetInfoOrThrow(string symbol)
        {
            var info = GetInfo(symbol);
            if (info == null)
                throw SymbolNotDefinedException.Create(symbol);
            return info;
        }

        public bool ContainsSymbol(string symbol)
        {
            if (_symbols.ContainsKey(symbol))
                return true;
            return _parent?.ContainsSymbol(symbol) ?? false;
        }

        public void AddSymbol(string symbol, SymbolInfo info)
        {
            if (ContainsSymbol(symbol))
                throw SymbolAlreadyDefinedException.Create(symbol);
            _symbols.Add(symbol, info);
        }
    }

    public class SymbolInfo
    {
        public Location DefinedAt { get; set; }
        public string DataType { get; set; }
    }
}
