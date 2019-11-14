using System;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Produce a single output and consume no input
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class ProduceParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<ISequence<TInput>, TOutput> _produce;

        public ProduceParser(Func<ISequence<TInput>, TOutput> produce)
        {
            _produce = produce;
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => new SuccessResult<object>(_produce(t));

        public IParseResult<TOutput> Parse(ISequence<TInput> t) => new SuccessResult<TOutput>(_produce(t));
    }
}