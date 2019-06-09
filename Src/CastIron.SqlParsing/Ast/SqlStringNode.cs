﻿using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStringNode : SqlNode
    {
        public SqlStringNode()
        {
        }

        public SqlStringNode(SqlToken token)
        {
            Value = token.Value;
            Location = token.Location;
        }

        public SqlStringNode(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("'");
            sb.Append(Value.Replace("'", "''"));
            sb.Append("'");
        }
    }
}