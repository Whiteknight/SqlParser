﻿using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlNullNode : SqlNode
    {
        public SqlNullNode()
        {
        }

        public SqlNullNode(SqlToken t)
        {
            Location = t.Location;
        }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("NULL");
        }
    }
}