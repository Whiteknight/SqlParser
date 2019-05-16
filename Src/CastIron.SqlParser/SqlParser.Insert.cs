using System;
using System.Collections.Generic;
using System.Text;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseInsertStatement(SqlTokenizer t)
        {
            t.Expect(SqlTokenType.Keyword, "INSERT");
            t.Expect(SqlTokenType.Keyword, "INTO");
            return new SqlNullNode();
        }
    }
}
