using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using SqlParser.Ast;

namespace SqlParser.Symbols
{
    public class SymbolTable
    {
        private readonly SymbolTable _parent;
        private readonly Dictionary<string, SymbolInfo> _symbols;

        public SymbolTable(IEnumerable<SymbolInfo> environmentalSymbols)
        {
            _symbols = new Dictionary<string, SymbolInfo>();
            foreach (var sym in (environmentalSymbols ?? Enumerable.Empty<SymbolInfo>()))
            {
                if (string.IsNullOrEmpty(sym.OriginalName))
                    continue;
                if (sym.OriginKind == SymbolOriginKind.Unknown)
                    sym.OriginKind = SymbolOriginKind.Environmental;
                _symbols.Add(sym.OriginalName, sym);
            }
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

        public SymbolInfo GetInfoOrThrow(string symbol, Location l)
        {
            var info = GetInfo(symbol);
            if (info == null)
                throw SymbolNotDefinedException.Create(symbol, l);
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
                throw SymbolAlreadyDefinedException.Create(symbol, info.DefinedAt);
            _symbols.Add(symbol, info);
        }

        public ISqlNode ResolveIdentifier(ISqlNode n)
        {
            if (n is SqlIdentifierNode id)
                return GetInfo(id.Name)?.Translate(n) ?? n;
            if (n is SqlObjectIdentifierNode oid)
                return GetInfo(oid.ToString())?.Translate(n) ?? n;
            if (n is SqlQualifiedIdentifierNode qid)
                return GetInfo(qid.ToString())?.Translate(n) ?? n;
            if (n is SqlVariableNode v)
                return GetInfo(v.Name)?.Translate(n) ?? GetInfo(v.GetBareName())?.Translate(n) ?? n;

            return n;
        }
    }
}
