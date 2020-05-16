using System;
using System.Runtime.Serialization;
using ParserObjects;

namespace SqlParser.Symbols
{
    [Serializable]
    public class SymbolWrongObjectKindException : Exception
    {
        public SymbolWrongObjectKindException()
        {
        }

        public SymbolWrongObjectKindException(string message) : base(message)
        {
        }

        public SymbolWrongObjectKindException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SymbolWrongObjectKindException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static SymbolWrongObjectKindException NonInvokableInvoked(SymbolInfo symbol, Location location)
        {
            return new SymbolWrongObjectKindException($"Symbol {symbol.OriginalName} is kind {symbol.ObjectKind} but is invoked like a function at {location}");
        }

        public static SymbolWrongObjectKindException NonDataSourceMemberAccessed(SymbolInfo symbol, Location location)
        {
            return new SymbolWrongObjectKindException($"Symbol {symbol.OriginalName} is kind {symbol.ObjectKind} but a member is accessed like a data source at {location}");
        }

        public static SymbolWrongObjectKindException NonScalarUsedInExpression(SymbolInfo symbol, Location location)
        {
            return new SymbolWrongObjectKindException($"Symbol {symbol.OriginalName} is kind {symbol.ObjectKind} but is used like a scalar value at {location}");
        }

        public static SymbolWrongObjectKindException NonTableUsedAsTableExpression(SymbolInfo symbol, Location location)
        {
            return new SymbolWrongObjectKindException($"Symbol {symbol.OriginalName} is kind {symbol.ObjectKind} but is used like a table expression at {location}");
        }
    }
}