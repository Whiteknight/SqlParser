using System;
using System.Collections.Generic;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Execute a parser repeatedly until it fails. Return a list of all produced items.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public class ListParser<TInput, TItem, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IParser<TInput, TItem> _parser;
        private readonly Func<IReadOnlyList<TItem>, TOutput> _produce;
        private readonly bool _atLeastOne;

        public ListParser(IParser<TInput, TItem> parser, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne)
        {
            _parser = parser;
            _produce = produce;
            _atLeastOne = atLeastOne;
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var items = new List<TItem>();
            while (true)
            {
                var result = _parser.Parse(t);
                if (!result.Success)
                    break;
                items.Add(result.Value);
            }

            return _atLeastOne && items.Count == 0 ? (IParseResult<TOutput>)new FailResult<TOutput>() : new SuccessResult<TOutput>(_produce(items));
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();
    }
}