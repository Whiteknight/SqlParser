using System;
using System.Collections.Generic;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tokenizing
{
    public class TokenScanner : ITokenScanner
    {
        private readonly ICharacterSequence _chars;
        private readonly SymbolSequence _operators;

        public TokenScanner(ICharacterSequence chars)
        {
            _chars = chars ?? throw new ArgumentNullException(nameof(chars));
            _operators = new SymbolSequence();

            // punctuation
            _operators.Add(".", ",", ";");

            // Parens
            _operators.Add("(", ")");

            // Arithmetic operators
            _operators.Add("+", "-", "/", "*", "&", "|", "^");

            // Unary ~. Unary - and + are covered above
            _operators.Add("~");

            // "=" is for comparison, ":=" is for assignment
            _operators.Add("=", ":=");

            // Comparison operators
            _operators.Add("!=", "<>", ">", "<", ">=", "<=");
        }

        public SqlToken ParseNext()
        {
            var c = _chars.Peek();
            if (c == '\0')
                return SqlToken.EndOfInput();

            if (char.IsWhiteSpace(c))
                return ReadWhitespace();
            if (char.IsNumber(c))
                return ReadNumber();
            if (char.IsLetter(c) || c == '_')
                return ReadWord();
            if (c == '$' || c == '#')
                return ReadSpecialIdentifier();
            if (c == '@')
                return ReadVariableName();
            if (c == '\'')
                return ReadQuoted();
            if (c == '"')
                return ReadQuotedIdentifier();
            if (c == '-')
            {
                _chars.GetNext();
                if (_chars.Peek() == '-')
                    return ReadSingleLineComment();

                _chars.PutBack(c);
                // Fall through, in case we're using '-' for some other purpose
            }
            if (c == '/')
            {
                _chars.GetNext();
                if (_chars.Peek() == '*')
                    return ReadMultilineComment();

                _chars.PutBack(c);
                // Fall through, in case we're using '/' for some other purpose
            }

            if (char.IsPunctuation(c) || char.IsSymbol(c))
                return ReadOperator();

            throw ParsingException.UnexpectedCharacter(c, _chars.GetLocation());
        }

        private SqlToken ReadSingleLineComment()
        {
            var l = _chars.GetLocation();
            _chars.GetNext();
            var x = ReadLine();
            return SqlToken.Comment(x, l);
        }

        private SqlToken ReadOperator()
        {
            var l = _chars.GetLocation();
            var op = ReadOperator(_operators);
            return SqlToken.Symbol(op, l);
        }

        private string ReadOperator(SymbolSequence op)
        {
            var x = _chars.GetNext();
            if (!char.IsPunctuation(x) && !char.IsSymbol(x))
            {
                _chars.PutBack(x);
                return op.Operator;
            }
            var nextOp = op.Get(x);
            if (nextOp == null)
            {
                _chars.PutBack(x);
                return op.Operator;
            }

            var recurse = ReadOperator(nextOp);
            if (recurse != null)
                return recurse;
            _chars.PutBack(x);
            if (!string.IsNullOrEmpty(op.Operator))
                return op.Operator;
            return null;
        }

        private SqlToken ReadMultilineComment()
        {
            var l = _chars.GetLocation();
            _chars.Expect('*');
            var chars = new List<char>();
            while (true)
            {
                var c = _chars.GetNext();
                if (c == '\0')
                {
                    _chars.PutBack(c);
                    break;
                }
                if (c == '*')
                {
                    if (_chars.Peek() == '/')
                    {
                        _chars.GetNext();
                        break;
                    }
                }

                chars.Add(c);
            }

            var x = new string(chars.ToArray());
            return SqlToken.Comment(x, l);
        }

        private string ReadLine()
        {
            var chars = new List<char>();
            while (true)
            {
                var c = _chars.GetNext();
                if (c == '\r' || c == '\n' || c == '\0')
                {
                    _chars.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            return new string(chars.ToArray());
        }

        private SqlToken ReadQuoted()
        {
            var l = _chars.GetLocation();
            var chars = new List<char>();
            _chars.Expect('\'');

            while (true)
            {
                var c = _chars.GetNext();
                if (c != '\'')
                {
                    chars.Add(c);
                    continue;
                }

                var n = _chars.GetNext();
                if (n == '\'')
                {
                    chars.Add(c);
                    continue;
                }

                _chars.PutBack(n);
                var x = new string(chars.ToArray());
                return SqlToken.QuotedString(x, l);
            }
        }

        private SqlToken ReadQuotedIdentifier()
        {
            var l = _chars.GetLocation();
            var chars = new List<char>();
            _chars.Expect('"');
            while (true)
            {
                var c = _chars.GetNext();
                if (c == '"')
                    break;
                chars.Add(c);
            }
            
            var x = new string(chars.ToArray());
            return SqlToken.QuotedIdentifier(x, l);
        }

        private SqlToken ReadNumber()
        {
            var l = _chars.GetLocation();
            var chars = new List<char>();
            while (true)
            {
                var c = _chars.GetNext();
                if (!char.IsNumber(c))
                {
                    _chars.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            if (_chars.Peek() == '.')
            {
                chars.Add(_chars.GetNext());
                while (true)
                {
                    var c = _chars.GetNext();
                    if (!char.IsNumber(c))
                    {
                        _chars.PutBack(c);
                        break;
                    }

                    chars.Add(c);
                }
            }

            var n = new string(chars.ToArray());
            return SqlToken.Number(n, l);
        }

        private SqlToken ReadWord()
        {
            var l = _chars.GetLocation();
            var chars = new List<char>();
            while (true)
            {
                var c = _chars.GetNext();
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    _chars.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            var s = new string(chars.ToArray());
            if (Facts.IsKeyword(s))
                return new SqlToken(s.ToUpperInvariant(), SqlTokenType.Keyword, l);
            return new SqlToken(s.ToLowerInvariant(), SqlTokenType.Identifier, l);
        }

        private SqlToken ReadSpecialIdentifier()
        {
            var l = _chars.GetLocation();
            var chars = new List<char>();
            var n = _chars.GetNext();
            chars.Add(n);
            while (true)
            {
                var c = _chars.GetNext();
                if (!char.IsLetterOrDigit(c) && c != '_')
                {
                    _chars.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            var x = new string(chars.ToArray());
            return SqlToken.Word(x, l);
        }

        private SqlToken ReadVariableName()
        {
            // TODO: We want to handle things like @@IDENTITY and @$$errno
            var l = _chars.GetLocation();
            var chars = new List<char>();
            while (true)
            {
                var c = _chars.GetNext();
                if (char.IsLetterOrDigit(c) || c == '@' || c == '$' || c == '#' || c == '_')
                {
                    chars.Add(c);
                    continue;
                }

                _chars.PutBack(c);
                break;
            }

            var x = new string(chars.ToArray());

            return SqlToken.Variable(x, l);
        }

        private SqlToken ReadWhitespace()
        {
            var l = _chars.GetLocation();
            var chars = new List<char>();
            while (true)
            {
                var c = _chars.GetNext();
                if (!char.IsWhiteSpace(c))
                {
                    _chars.PutBack(c);
                    break;
                }

                chars.Add(c);
            }

            var s = new string(chars.ToArray());
            return SqlToken.Whitespace(s, l);
        }
    }
}