using System.Collections.Generic;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Given a list of possible alternatives, return the result of the first parser which succeeds
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class FirstParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IReadOnlyList<IParser<TInput, TOutput>> _parsers;

        public FirstParser(params IParser<TInput, TOutput>[] parsers)
        {
            _parsers = parsers;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            foreach (var parser in _parsers)
            {
                var result = parser.Parse(t);
                if (result.Success)
                    return result;
            }

            return new FailResult<TOutput>();
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}