namespace SqlParser.Tokenizing
{
    // TODO: Implementation to pull chars out of a file stream instead of a string
    public interface ICharacterSequence
    {
        void Expect(char expected);
        char Peek();
        char GetNext();
        void PutBack(char c);
        Location GetLocation();
    }
}