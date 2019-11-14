using System;

namespace SqlParser.Parsing.Parsers
{
    public class ApplyParser<TInput, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IParser<TInput, TOutput> _initial;
        private readonly IParser<TInput, TOutput> _right;
        private readonly LeftParser _left;

        public ApplyParser(IParser<TInput, TOutput> initial, Func<IParser<TInput, TOutput>, IParser<TInput, TOutput>> getRight)
        {
            _initial = initial;
            _left = new LeftParser();
            _right = getRight(_left);
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var leftResult = _initial.Parse(t);
            if (!leftResult.Success)
                return new FailResult<TOutput>();

            _left.Value = leftResult.Value;
            var rhsResult = _right.Parse(t);
            if (!rhsResult.Success)
                return leftResult;

            return rhsResult;
        }

        IParseResult<object> IParser<TInput>.ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();

        private class LeftParser : IParser<TInput, TOutput>
        {
            public TOutput Value { get; set; }

            public IParseResult<TOutput> Parse(ISequence<TInput> t) => new SuccessResult<TOutput>(Value);

            IParseResult<object> IParser<TInput>.ParseUntyped(ISequence<TInput> t) => new SuccessResult<object>(Value);
        }
    }

    public class ApplyParser<TInput, TMiddle, TOutput> : IParser<TInput, TOutput>
    {
        private readonly IParser<TInput, TMiddle> _initial;
        private readonly IParser<TInput, TOutput> _right;
        private readonly LeftParser _left;

        public ApplyParser(IParser<TInput, TMiddle> initial, Func<IParser<TInput, TMiddle>, IParser<TInput, TOutput>> getRight)
        {
            _initial = initial;
            _left = new LeftParser();
            _right = getRight(_left);
        }

        public IParseResult<TOutput> Parse(ISequence<TInput> t)
        {
            var window = new WindowSequence<TInput>(t);
            var leftResult = _initial.Parse(window);
            if (!leftResult.Success)
                return new FailResult<TOutput>();

            _left.Value = leftResult.Value;
            var rhsResult = _right.Parse(window);
            if (!rhsResult.Success)
                window.Rewind();

            return rhsResult;
        }

        IParseResult<object> IParser<TInput>.ParseUntyped(ISequence<TInput> t) => Parse(t).Untype();

        private class LeftParser : IParser<TInput, TMiddle>
        {
            public TMiddle Value { get; set; }

            public IParseResult<TMiddle> Parse(ISequence<TInput> t) => new SuccessResult<TMiddle>(Value);

            IParseResult<object> IParser<TInput>.ParseUntyped(ISequence<TInput> t) => new SuccessResult<object>(Value);
        }
    }
}