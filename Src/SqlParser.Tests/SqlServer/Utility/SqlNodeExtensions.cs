﻿using SqlParser.Ast;

namespace SqlParser.Tests.SqlServer.Utility
{
    public static class SqlNodeExtensions
    {
        public static SqlNodeAssertions Should(this ISqlNode node)
        {
            return new SqlNodeAssertions(node);
        }
    }
}