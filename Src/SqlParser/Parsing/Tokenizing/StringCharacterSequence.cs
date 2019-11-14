using System.Collections.Generic;
using SqlParser.Parsing;

namespace SqlParser.Tokenizing
{
    public class StringCharacterSequence : ISequence<char>
    {
        private readonly string _s;
        private readonly Stack<char> _putbacks;
        private int _current;
        private int _line;
        private int _lineChar;

        public StringCharacterSequence(string s)
        {
            _s = s;
            _current = 0;
            _putbacks = new Stack<char>();
            _line = 1;
            _lineChar = 0;
        }

        public void Expect(char expected)
        {
            var found = GetNext();
            if (found != expected)
                throw ParsingException.UnexpectedCharacter(expected, found, CurrentLocation);
        }

        public char Peek()
        {
            var c = GetNext();
            PutBack(c);
            return c;
        }

        public bool IsAtEnd => _putbacks.Count == 0 && _current >= _s.Length;
        public Location CurrentLocation => new Location(_line, _lineChar);

        public char GetNext()
        {
            if (_putbacks.Count > 0)
                return _putbacks.Pop();
            if (_current >= _s.Length)
                return '\0';
            var next = _s[_current++];
            if (next == '\n')
            {
                _line++;
                _lineChar = -1;
            }
            else
                _lineChar++;

            return next;
        }

        public void PutBack(char c)
        {
            if (c == '\n')
                _line--;
            _putbacks.Push(c);
        }
    }
}