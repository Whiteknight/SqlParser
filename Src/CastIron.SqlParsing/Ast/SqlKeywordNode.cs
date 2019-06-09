﻿using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlKeywordNode : SqlNode
    {
        public SqlKeywordNode()
        {
        }

        public SqlKeywordNode(SqlToken t)
        {
            Keyword = t.Value;
            Location = t.Location;
        }

        public SqlKeywordNode(string keyword)
        {
            Keyword = keyword;
        }

        public string Keyword { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append(Keyword);
        }
    }
}