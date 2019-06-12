﻿using System.Text;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public abstract class SqlNode
    {
        public abstract SqlNode Accept(SqlNodeVisitor visitor);

        public override string ToString()
        {
            var sb = new StringBuilder();
            var visitor = new SqlStringifyVisitor(sb);
            visitor.Visit(this);
            return sb.ToString();
        }

        public Location Location { get; set; }
    }
}