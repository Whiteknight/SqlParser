using System;
using System.Collections;
using System.Collections.Generic;

namespace SqlParser.Tokenizing
{
    public class Tokenizer : ITokenizer
    {
        private readonly Stack<SqlToken> _putbacks;
        private readonly ITokenScanner _scanner;

        public Tokenizer(ITokenScanner scanner)
        {
            _putbacks = new Stack<SqlToken>();
            _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        }

        public static ITokenizer ForSqlServer(string s)
        {
            return new Tokenizer(new SqlParser.SqlServer.Tokenizing.TokenScanner(new StringCharacterSequence(s)));
        }

        public static ITokenizer ForPostgreSql(string s)
        {
            return new Tokenizer(new SqlParser.PostgreSql.Tokenizing.TokenScanner(new StringCharacterSequence(s)));
        }

        public SqlToken GetNextToken()
        {
            if (_putbacks.Count > 0)
                return _putbacks.Pop();
            
            var next = _scanner.ParseNext();
            if (next == null)
                return SqlToken.EndOfInput();
            return next;
        }



        public void PutBack(SqlToken token)
        {
            if (token != null)
                _putbacks.Push(token);
        }

        public IEnumerator<SqlToken> GetEnumerator()
        {
            while (true)
            {
                var next = _scanner.ParseNext();
                if (next == null || next.IsType(SqlTokenType.EndOfInput))
                    break;
                yield return next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}