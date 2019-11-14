namespace SqlParser.Parsing
{
    /// <summary>
    /// Parser result objects, contains a success/fail flag and the production value
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public interface IParseResult<out TOutput>
    {
        bool Success { get; }
        TOutput Value { get; }

        /// <summary>
        /// Convert the return value explicitly to object
        /// (cast to IParseResult of object fails when TOutput is a struct/primitive type and boxing is involved)
        /// </summary>
        /// <returns></returns>
        IParseResult<object> Untype();
    }

    /// <summary>
    /// Represents a parse failure
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public struct FailResult<TOutput> : IParseResult<TOutput>
    {
        public bool Success => false;
        public TOutput Value => default;

        public IParseResult<object> Untype() => new FailResult<object>();
    }

    /// <summary>
    /// Represents a successful parse production
    /// </summary>
    /// <typeparam name="TOutput"></typeparam>
    public struct SuccessResult<TOutput> : IParseResult<TOutput>
    {
        public SuccessResult(TOutput value)
        {
            Value = value;
        }

        public bool Success => true;
        public TOutput Value { get; }

        public IParseResult<object> Untype() => new SuccessResult<object>(Value);
    }
}