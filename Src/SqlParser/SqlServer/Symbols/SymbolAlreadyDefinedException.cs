using System;

namespace SqlParser.SqlServer.Symbols
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


        public string Symbol { get; set; }

        public static SymbolAlreadyDefinedException Create(string symbol)
        {
            return new SymbolAlreadyDefinedException($"Symbol {symbol} already defined. Cannot add duplicate definition.")
            {
                Symbol = symbol
            };
        }
    }
}