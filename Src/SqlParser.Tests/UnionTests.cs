﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests
{
    [TestFixture]
    public class UnionTests
    {
        [TestCase("UNION")]
        [TestCase("UNION ALL")]
        [TestCase("EXCEPT")]
        [TestCase("INTERSECT")]
        public void Select_UnionOperatorSelect(string op)
        {
            string s = $"SELECT * FROM Table1 {op} SELECT * FROM Table2";
            var target = new Parser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();
            var o1 = result.ToString();

            result.Statements.First().Should().MatchAst(
                new SqlInfixOperationNode
                {
                    Left = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            }
                        },
                        FromClause = new SqlObjectIdentifierNode("Table1")
                    },
                    Operator = new SqlOperatorNode(op),
                    Right = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            }
                        },
                        FromClause = new SqlObjectIdentifierNode("Table2")
                    }
                }
            );
        }

        [Test]
        public void Select_UnionSelect2x()
        {
            const string s = "SELECT * FROM Table1 UNION SELECT * FROM Table2 UNION ALL SELECT * FROM Table3;";
            var target = new Parser();
            var result = target.Parse(new SqlTokenizer(s));
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInfixOperationNode
                {
                    Left = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            }
                        },
                        FromClause = new SqlObjectIdentifierNode("Table1")
                    },
                    Operator = new SqlOperatorNode("UNION"),
                    Right = new SqlInfixOperationNode
                    {
                        Left = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                Children = new List<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                }
                            },
                            FromClause = new SqlObjectIdentifierNode("Table2")
                        },
                        Operator = new SqlOperatorNode("UNION ALL"),
                        Right = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                Children = new List<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                }
                            },
                            FromClause = new SqlObjectIdentifierNode("Table3")
                        }
                    }
                }
            );
        }
    }
}