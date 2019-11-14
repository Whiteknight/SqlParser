namespace SqlParser.Parsing
{
    public interface IParser
    {
    }

    /// <summary>
    /// Parser class takes a sequence of TInput and outputs productions of unknown type
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IParser<TInput> : IParser
    {
        IParseResult<object> ParseUntyped(ISequence<TInput> t);
    }

    /// <summary>
    /// Parser class takes a sequence of TInput and outputs productions of type TOutput
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IParser<TInput, out TOutput> : IParser<TInput>
    {
        IParseResult<TOutput> Parse(ISequence<TInput> t);
    }
}
