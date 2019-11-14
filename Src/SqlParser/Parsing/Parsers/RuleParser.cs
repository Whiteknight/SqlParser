using System;
using System.Collections.Generic;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Attempts to execute a series of parsers in order, one after the other. If any fails, the entire
    /// production fails
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class RuleParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IReadOnlyList<IParser<TInput>> _parsers;
        private readonly Func<IReadOnlyList<object>, TOutput> _produce;

        public RuleParser(IReadOnlyList<IParser<TInput>> parsers, Func<IReadOnlyList<object>, TOutput> produce)
        {
            _parsers = parsers;
            _produce = produce;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var window = new WindowSequence<TInput>(t);
            var outputs = new object[_parsers.Count];
            for (var i = 0; i < _parsers.Count; i++)
            {
                var result = _parsers[i].ParseUntyped(window);
                if (!result.Success)
                {
                    window.Rewind();
                    return new FailResult<TOutput>();
                }

                outputs[i] = result.Value;
            }

            return new SuccessResult<TOutput>(_produce(outputs));
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}