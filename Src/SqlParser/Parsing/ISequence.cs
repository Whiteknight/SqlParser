namespace SqlParser.Parsing
{
    /// <summary>
    /// Iterator pattern implementation with putback
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISequence<T>
    {
        T GetNext();
        void PutBack(T c);
        T Peek();
        bool IsAtEnd { get; }
        Location CurrentLocation { get; }
    }
}