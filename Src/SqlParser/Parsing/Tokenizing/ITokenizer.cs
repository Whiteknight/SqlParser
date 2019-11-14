using System.Collections.Generic;

namespace SqlParser.Tokenizing
{
    public interface ITokenizer : IEnumerable<SqlToken>
    {
        SqlToken ScanNext();
        void PutBack(SqlToken token);
    }

    public interface ITokenScanner
    {
        SqlToken ParseNext();
    }
}