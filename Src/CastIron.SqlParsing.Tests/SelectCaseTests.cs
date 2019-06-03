﻿using System.Collections.Generic;
using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectCaseTests
    { 

        [Test]
        public void Select_CaseWhenThenElseEnd()
        {
            const string s = "SELECT CASE 5 WHEN 6 THEN 'A' ELSE 'B' END;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();
            var output = result.ToString();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlCaseNode
                        {
                            InputExpression = new SqlNumberNode(5),
                            WhenExpressions = new List<SqlCaseWhenNode>
                            {
                                new SqlCaseWhenNode
                                {
                                    Condition = new SqlNumberNode(6),
                                    Result = new SqlStringNode("A")
                                }
                            },
                            ElseExpression = new SqlStringNode("B")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_CaseWhenThenElseEndAlias()
        {
            const string s = "SELECT CASE ValueA WHEN 6 THEN 'A' ELSE 'B' END AS ColumnA, ColumnB;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("ColumnA"),
                            Source = new SqlCaseNode
                            {
                                InputExpression = new SqlIdentifierNode("ValueA"),
                                WhenExpressions = new List<SqlCaseWhenNode>
                                {
                                    new SqlCaseWhenNode
                                    {
                                        Condition = new SqlNumberNode(6),
                                        Result = new SqlStringNode("A")
                                    }
                                },
                                ElseExpression = new SqlStringNode("B")
                            }
                        },
                        new SqlIdentifierNode("ColumnB")
                    }
                }
            );
        }
    }
}