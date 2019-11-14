using System;

namespace SqlParser.Parsing.Parsers
{
    /// <summary>
    /// Transforms the successfull output from one parser into another type
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    /// <typeparam name="TTransform"></typeparam>
    public class TransformParser<TInput, TOutput, TTransform> : IParser<TInput, TTransform>
    {
        private readonly IParser<TInput, TOutput> _parser;
        private readonly Func<TOutput, TTransform> _produce;

        public TransformParser(IParser<TInput, TOutput> parser, Func<TOutput, TTransform> produce)
        {
            _parser = parser;
            _produce = produce;
        }

        public IParseResult<object> ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();

        public IParseResult<TTransform> Parse(ISequence<TInput> t)
        {
            var result = _parser.Parse(t);
            return result.Success ? new SuccessResult<TTransform>(_produce(result.Value)) : (IParseResult<TTransform>)new FailResult<TTransform>();
        }
    }
}