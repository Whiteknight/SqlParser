﻿namespace SqlParser.SqlStandard.Tests.Utility
{
    public static class SqlNodeExtensions
    {
        public static SqlNodeAssertions Should(this ISqlNode node)
        {
            return new SqlNodeAssertions(node);
        }
    }
}