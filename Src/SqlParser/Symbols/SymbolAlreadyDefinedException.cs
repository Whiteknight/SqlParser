using System;
using System.Runtime.Serialization;
using ParserObjects;

namespace SqlParser.Symbols
{
    [Serializable]
    public class SymbolAlreadyDefinedException : Exception
    {
        public SymbolAlreadyDefinedException()
        {
        }

        public SymbolAlreadyDefinedException(string message) : base(message)
        {
        }

        public SymbolAlreadyDefinedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SymbolAlreadyDefinedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public string Symbol { get; private set; }
        public Location Location { get; private set; }

        public static SymbolAlreadyDefinedException Create(string symbol, Location l)
        {
            return new SymbolAlreadyDefinedException($"Symbol {symbol} already defined. Cannot add duplicate definition at {l}.")
            {
                Symbol = symbol,
                Location = l
            };
        }
    }
}