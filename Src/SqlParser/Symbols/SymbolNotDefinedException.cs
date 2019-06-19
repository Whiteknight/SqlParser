using System;
using System.Runtime.Serialization;

namespace SqlParser.Symbols
{
    [Serializable]
    public class SymbolNotDefinedException : Exception
    {
        public SymbolNotDefinedException()
        {
        }

        public SymbolNotDefinedException(string message) : base(message)
        {
        }

        public SymbolNotDefinedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SymbolNotDefinedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public string Symbol { get; set; }

        public static SymbolNotDefinedException Create(string symbol)
        {
            return new SymbolNotDefinedException($"Cannot find symbol {symbol}.")
            {
                Symbol = symbol
            };
        }
    }
}