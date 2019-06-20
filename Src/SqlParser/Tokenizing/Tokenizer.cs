﻿using System.Collections;
using System.Collections.Generic;

namespace SqlParser.Tokenizing
{
    public class Tokenizer : IEnumerable<SqlToken>
    {
        private readonly IEnumerator<SqlToken> _enumerator;
        private readonly Stack<SqlToken> _putbacks;
        private readonly SymbolSequence _operators;

        public Tokenizer(string s)
        {
            _putbacks = new Stack<SqlToken>();

            _operators = new SymbolSequence();

            // punctuation
            _operators.Add(".", ",", ";");

            // Parens
            _operators.Add("(", ")");

            // Arithmetic operators
            _operators.Add("+", "-", "/", "*", "&", "|", "^");

            // Unary ~. Unary - and + are covered above
            _operators.Add("~");

            // "=" is both for comparison and assignment
            _operators.Add("=");

            // Comparison operators
            _operators.Add("!=", "<>", ">", "<", ">=", "<=");

            // Assignment operators
            _operators.Add("+=", "-=", "*=", "/=", "%=", "&=", "^=", "|=");

            _enumerator = GetEnumerator(new CharSequence(s ?? ""));
        }

        public SqlToken GetNext(bool skipComments = true)
        {
            if (_putbacks.Count > 0)
                return _putbacks.Pop();
            while (true)
            {
                if (!_enumerator.MoveNext())
                    return SqlToken.EndOfInput();
                var next = _enumerator.Current;
                if (next == null)
                    return SqlToken.EndOfInput();
                if (next.Type == SqlTokenType.Comment && skipComments)
                    continue;
                if (next.Type == SqlTokenType.Whitespace)
                    continue;
                return next;
            }
        }

        public bool NextIs(SqlTokenType type, string value, bool consume = false)
        {
            var t = GetNext();
            bool isSame = t.Type == type && t.Value == value;
            if (!isSame)
            {
                PutBack(t);
                return false;
            }
            if (!consume)
                PutBack(t);
            return true;
        }

        public void PutBack(SqlToken token)
        {
            if (token != null)
                _putbacks.Push(token);
        }

        public SqlToken Expect(SqlTokenType type)
        {
            var found = GetNext();
            if (found.Type != type)
                throw ParsingException.UnexpectedToken(type, found);
            return found;
        }

        public SqlToken Expect(SqlTokenType type, params string[] values)
        {
            var found = GetNext();
            if (found.Type == type)
            {
                foreach (var value in values)
                {
                    if (found.Value == value)
                        return found;
                }
            }

            throw ParsingException.UnexpectedToken(type, values, found);
        }

        public SqlToken Peek()
        {
            var t = GetNext();
            PutBack(t);
            return t;
        }

        public SqlToken ExpectPeek(SqlTokenType type)
        {
            var found = Peek();
            if (found.Type != type)
                throw ParsingException.UnexpectedToken(type, found);
            return found;
        }

        public void Skip(SqlTokenType type)
        {
            while (true)
            {
                var t = GetNext();
                if (t.Type == SqlTokenType.EndOfInput || type == SqlTokenType.EndOfInput)
                    break;
                if (t.Type != type)
                {
                    PutBack(t);
                    break;
                }
            }
        }

        private class SymbolSequence
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

        private IEnumerator<SqlToken> GetEnumerator(CharSequence s)
        {
            while (true)
            {
                var c = s.Peek();
                if (c == '\0')
                {
                    yield return SqlToken.EndOfInput();
                    yield break;
                }
                if (char.IsWhiteSpace(c))
                {
                    var l = s.GetLocation();
                    var w = ReadWhitespace(s);
                    yield return SqlToken.Whitespace(w, l);
                    continue;
                }

                if (char.IsNumber(c))
                {
                    var l = s.GetLocation();
                    var n = ReadNumber(s);
                    yield return SqlToken.Number(n, l);
                    continue;
                }
                if (char.IsLetter(c))
                {
                    var l = s.GetLocation();
                    var x = ReadWord(s);
                    yield return SqlToken.Word(x, l);
                    continue;
                }
                if (c == '$' || c == '#')
                {
                    var l = s.GetLocation();
                    var n = s.GetNext();
                    var x = ReadWord(s);
                    yield return SqlToken.Word(c + x, l);
                    continue;
                }
                if (c == '@')
                {
                    // TODO: We want to handle things like @@IDENTITY and @$$errno
                    var l = s.GetLocation();
                    s.GetNext();
                    var x = ReadVariableName(s);
                    yield return SqlToken.Variable(x, l);
                    continue;
                }
                if (c == '\'')
                {
                    var x = ReadQuoted(s);
                    var l = s.GetLocation();
                    yield return SqlToken.QuotedString(x, l);
                    continue;
                }
                if (c == '[')
                {
                    var l = s.GetLocation();
                    var x = ReadBracketed(s);
                    yield return SqlToken.BracketedIdentifier(x, l);
                    continue;
                }
                if (c == '-')
                {
                    s.GetNext();
                    if (s.Peek() == '-')
                    {
                        var l = s.GetLocation();
                        s.GetNext();
                        var x = ReadLine(s);
                        yield return SqlToken.Comment(x, l);
                        continue;
                    }

                    s.PutBack(c);
                    // Fall through, in case we're using '-' for some other purpose
                }
                if (c == '/')
                {
                    s.GetNext();
                    if (s.Peek() == '*')
                    {
                        s.GetNext();
                        var l = s.GetLocation();
                        var x = ReadMultilineComment(s);
                        yield return SqlToken.Comment(x, l);
                        continue;
                    }

                    s.PutBack(c);
                    // Fall through, in case we're using '/' for some other purpose
                }
                if (char.IsPunctuation(c) || char.IsSymbol(c))
                {
                    var l = s.GetLocation();
                    var op = ReadOperator(s);
                    yield return SqlToken.Symbol(op, l);
                    continue;
                }
            }
        }

        private string ReadOperator(CharSequence s)
        {
            return ReadOperator(s, _operators);
        }

        private string ReadOperator(CharSequence s, SymbolSequence op)
        {
            var x = s.GetNext();
            if (!char.IsPunctuation(x) && !char.IsSymbol(x))
            {
                s.PutBack(x);
                return op.Operator;
            }
            var nextOp = op.Get(x);
            if (nextOp == null)
            {
                s.PutBack(x);
                return op.Operator;
            }

            var recurse = ReadOperator(s, nextOp);
            if (recurse != null)
                return recurse;
            s.PutBack(x);
            if (!string.IsNullOrEmpty(op.Operator))
                return op.Operator;
            return null;
        }

        private string ReadMultilineComment(CharSequence s)
        {
            var chars = new List<char>();
            while (true)
            {
                var c = s.GetNext();
                if (c == '\0')
                {
                    s.PutBack(c);
                    break;
                }
                if (c == '*')
                {
                    if (s.Peek() == '/')
                    {
                        s.GetNext();
                        break;
                    }
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private string ReadLine(CharSequence s)
        {
            var chars = new List<char>();
            while (true)
            {
                var c = s.GetNext();
                if (c == '\r' || c == '\n' || c == '\0')
                {
                    s.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private string ReadQuoted(CharSequence s)
        {
            var chars = new List<char>();
            s.Expect('\'');

            while (true)
            {
                var c = s.GetNext();
                if (c != '\'')
                {
                    chars.Add(c);
                    continue;
                }

                var n = s.GetNext();
                if (n == '\'')
                {
                    chars.Add(c);
                    continue;
                }

                s.PutBack(n);
                return (new string(chars.ToArray()));
            }
        }

        private string ReadBracketed(CharSequence s)
        {
            var chars = new List<char>();
            s.Expect('[');
            while (true)
            {
                var c = s.GetNext();
                if (c == ']')
                    return new string(chars.ToArray());
                chars.Add(c);
            }
        }

        private string ReadNumber(CharSequence s)
        {
            var chars = new List<char>();
            while (true)
            {
                var c = s.GetNext();
                if (!char.IsNumber(c))
                {
                    s.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            if (s.Peek() != '.')
                return new string(chars.ToArray());

            chars.Add(s.GetNext());
            while (true)
            {
                var c = s.GetNext();
                if (!char.IsNumber(c))
                {
                    s.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private string ReadWord(CharSequence s)
        {
            var chars = new List<char>();
            while (true)
            {
                var c = s.GetNext();
                if (!char.IsLetterOrDigit(c))
                {
                    s.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private string ReadVariableName(CharSequence s)
        {
            var chars = new List<char>();
            chars.Add('@');
            while (true)
            {
                var c = s.GetNext();
                if (char.IsLetterOrDigit(c) || c == '@' || c == '$' || c == '#' || c == '_')
                {
                    chars.Add(c);
                    continue;
                }

                s.PutBack(c);
                break;
            }

            return new string(chars.ToArray());
        }

        private string ReadWhitespace(CharSequence s)
        {
            var chars = new List<char>();
            while (true)
            {
                var c = s.GetNext();
                if (!char.IsWhiteSpace(c))
                {
                    s.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        public IEnumerator<SqlToken> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}