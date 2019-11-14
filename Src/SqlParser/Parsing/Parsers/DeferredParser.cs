using System;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Get a parser reference dynamically. Used to resolve circular references in the parser graph
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class DeferredParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<IParser<TInput, TOutput>> _getParser;

        public DeferredParser(Func<IParser<TInput, TOutput>> getParser)
        {
            _getParser = getParser;
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => _getParser().ParseUntyped(t);

        public IParseResult<TOutput> Parse(ISequence<TInput> t) => _getParser().Parse(t);
    }
}