using System.Collections.Generic;

namespace SqlParser.Tokenizing
{
    public class SymbolSequence
    {
        private readonly Dictionary<char, SymbolSequence> _next;
        public string Operator { get; private set; }

        public SymbolSequence()
        {
            _next = new Dictionary<char, SymbolSequence>();
        }

        public void Add(params string[] chars)
        {
            foreach (var c in chars)
                Add(c, 0);
        }

        private void Add(string chars, int idx)
        {
            bool isLast = idx == chars.Length;
            if (isLast)
            {
                Operator = chars;
                return;
            }
            var c = chars[idx];
            if (!_next.ContainsKey(c))
                _next.Add(c, new SymbolSequence());
            _next[c].Add(chars, idx + 1);
        }

        public SymbolSequence Get(char c)
        {
            if (_next.ContainsKey(c))
                return _next[c];
            return null;
        }
    }
}