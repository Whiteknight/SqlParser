using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class SelectWhereTests
    {
        [Test]
        public void Select_WhereColumnEqualsNumber()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn = 1;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(1)
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnEqualsExpression()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn1 = mycolumn2 + 1;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn1"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn2"),
                            Operator = new SqlOperatorNode("+"),
                            Right = new SqlNumberNode(1)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereAnd()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn1 = 1 AND mycolumn2 = 2;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn1"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        },
                        Operator = new SqlOperatorNode("AND"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn2"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(2)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnBetween()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn BETWEEN 1 AND 2;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlBetweenOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Low = new SqlNumberNode(1),
                        High = new SqlNumberNode(2)
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnInNumberList()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn IN (1, 2, 3);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInNode
                    {

                        Search = new SqlIdentifierNode("mycolumn"),
                        Items = new SqlListNode<ISqlNode>
                        {
                            new SqlNumberNode(1),
                            new SqlNumberNode(2),
                            new SqlNumberNode(3)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnIsNull()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn IS NULL;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Operator = new SqlOperatorNode("IS"),
                        Right = new SqlNullNode()
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnIsNotNull()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn IS NOT NULL;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Operator = new SqlOperatorNode("IS NOT"),
                        Right = new SqlNullNode()
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnLikeString()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn LIKE '%test%';";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Operator = new SqlOperatorNode("LIKE"),
                        Right = new SqlStringNode("%test%")
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnNotLikeString()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn NOT LIKE '%test%';";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Operator = new SqlOperatorNode("NOT LIKE"),
                        Right = new SqlStringNode("%test%")
                    }
                }
            );
        }

        [Test]
        public void Select_WhereExists()
        {
            const string s = "SELECT * FROM mytable1 WHERE EXISTS (SELECT * FROM mytable2);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable1"),
                    WhereClause = new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode("EXISTS"),
                        Right = new SqlParenthesisNode<ISqlNode>(new SqlSelectNode
                        {
                            Columns = new SqlListNode<ISqlNode>
                            {
                                new SqlOperatorNode("*")
                            },
                            FromClause = new SqlObjectIdentifierNode("mytable2")
                        })
                    }
                }
            );
        }

        [Test]
        public void Select_WhereNotExists()
        {
            const string s = "SELECT * FROM mytable1 WHERE NOT EXISTS (SELECT * FROM mytable2);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable1"),
                    WhereClause = new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode("NOT"),
                        Right = new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("EXISTS"),
                            Right = new SqlParenthesisNode<ISqlNode>(new SqlSelectNode
                            {
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("mytable2")
                            })
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereEqualsAllSelect()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn = ALL (SELECT 5);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("mycolumn"),
                        Operator = new SqlOperatorNode("= ALL"),
                        Right = new SqlParenthesisNode<ISqlNode>
                        {
                            Expression = new SqlSelectNode
                            {
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlNumberNode(5)
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereAndOr1()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn1 = 1 AND (mycolumn2 = 2 OR mycolumn3 = 3);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn1"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        },
                        Operator = new SqlOperatorNode("AND"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            },
                            Operator = new SqlOperatorNode("OR"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn3"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(3)
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereAndOr2()
        {
            const string s = "SELECT * FROM mytable WHERE mycolumn1 = 1 AND mycolumn2 = 2 OR mycolumn3 = 3;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("AND"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            }
                        },
                        Operator = new SqlOperatorNode("OR"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn3"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(3)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereAndOr3()
        {
            const string s = "SELECT * FROM mytable WHERE (mycolumn1 = 1 AND mycolumn2 = 2) OR mycolumn3 = 3;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("AND"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            }
                        },
                        Operator = new SqlOperatorNode("OR"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn3"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(3)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_NotOr1()
        {
            const string s = "SELECT * FROM mytable WHERE NOT mycolumn1 = 1 OR mycolumn2 = 2;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("NOT"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            }
                        },
                        Operator = new SqlOperatorNode("OR"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("mycolumn2"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(2)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_NotOr2()
        {
            const string s = "SELECT * FROM mytable WHERE NOT (mycolumn1 = 1 OR mycolumn2 = 2);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("mytable"),
                    WhereClause = new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode("NOT"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("OR"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("mycolumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            }
                        }
                    }
                }
            );
        }
    }
}