using System;
using System.Runtime.Serialization;
using ParserObjects;

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

        public string Symbol { get; private set; }
        public Location Location { get; private set; }

        public static SymbolNotDefinedException Create(string symbol, Location l)
        {
            if (l == null)
            {
                return new SymbolNotDefinedException($"Symbol {symbol} is used but is not defined.")
                {
                    Symbol = symbol
                };
            }

            return new SymbolNotDefinedException($"Symbol {symbol} is used but is not defined at {l}.")
            {
                Symbol = symbol,
                Location = l
            };
        }
    }
}