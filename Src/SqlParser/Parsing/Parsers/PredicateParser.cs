using System;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Test a condition and produce an output if the condition succeeds.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class PredicateParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly Func<TInput, bool> _predicate;
        private readonly Func<TInput, TOutput> _produce;

        public PredicateParser(Func<TInput, bool> predicate, Func<TInput, TOutput> produce)
        {
            _predicate = predicate;
            _produce = produce;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
            => _predicate(t.Peek()) ? new SuccessResult<TOutput>(_produce(t.GetNext())) : (IParseResult<TOutput>)new FailResult<TOutput>();

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}