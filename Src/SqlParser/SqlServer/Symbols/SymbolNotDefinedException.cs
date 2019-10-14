using System;

namespace SqlParser.SqlServer.Symbols
{
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