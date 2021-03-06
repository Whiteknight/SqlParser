﻿using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class SelectGroupByTests
    {
        [Test]
        public void Select_GroupByColumn()
        {
            const string s = "SELECT * FROM \"TableA\" GROUP BY \"Column1\"";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("TableA"),
                    GroupByClause = new SqlListNode<ISqlNode>
                    {
                        new SqlIdentifierNode("Column1")
                    }
                }
            );
        }
    }
}