﻿using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SetTests
    {
        [Test]
        public void Set_Number()
        {
            const string s = "SET @var = 5;";
            var target = new SqlParser();
            var result = target.Parse(s);
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Right = new SqlNumberNode(5)
                }
            );
        }
    }
}