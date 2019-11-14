using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Convenience parser to help with matching an expected sequence of inputs
    /// Attempt to match the entire sequence specified by the input pattern or fail.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class MatchSequenceParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly TInput[] _find;
        private readonly Func<TInput[], TOutput> _produce;

        public MatchSequenceParser(IEnumerable<TInput> find, Func<TInput[], TOutput> produce)
        {
            _find = find.ToArray();
            _produce = produce;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            // small optimization, don't allocate a buffer if we only need one item
            if (_find.Length == 1)
                return t.Peek().Equals(_find[0]) ? new SuccessResult<TOutput>(_produce(new[] { t.GetNext() })) : (IParseResult<TOutput>)new FailResult<TOutput>();

            var window = new WindowSequence<TInput>(t);
            var buffer = new TInput[_find.Length];
            for (var i = 0; i < _find.Length; i++)
            {
                buffer[i] = window.GetNext();
                if (buffer[i].Equals(_find[i]))
                    continue;

                window.Rewind();
                return new FailResult<TOutput>();
            }

            return new SuccessResult<TOutput>(_produce(buffer));
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}