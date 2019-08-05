using System.Collections.Generic;

namespace SqlParser.Tokenizing
{
    public interface ITokenizer : IEnumerable<SqlToken>
    {
        SqlToken GetNextToken();
        void PutBack(SqlToken token);
    }
}