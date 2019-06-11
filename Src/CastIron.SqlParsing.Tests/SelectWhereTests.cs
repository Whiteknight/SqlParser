using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectWhereTests
    {
        [Test]
        public void Select_WhereColumnEqualsNumber()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn = 1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(1)
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnEqualsExpression()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn1 = MyColumn2 + 1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn1"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn2"),
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
            const string s = "SELECT * FROM MyTable WHERE MyColumn1 = 1 AND MyColumn2 = 2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn1"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        },
                        Operator = new SqlOperatorNode("AND"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn2"),
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
            const string s = "SELECT * FROM MyTable WHERE MyColumn BETWEEN 1 AND 2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlBetweenOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Low = new SqlNumberNode(1),
                        High = new SqlNumberNode(2)
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnInNumberList()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn IN (1, 2, 3);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInNode
                    {

                        Search = new SqlIdentifierNode("MyColumn"),
                        Items = new SqlListNode<SqlNode>
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
            const string s = "SELECT * FROM MyTable WHERE MyColumn IS NULL;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("IS"),
                        Right = new SqlNullNode()
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnIsNotNull()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn IS NOT NULL;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("IS NOT"),
                        Right = new SqlNullNode()
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnLikeString()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn LIKE '%test%';";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("LIKE"),
                        Right = new SqlStringNode("%test%")
                    }
                }
            );
        }

        [Test]
        public void Select_WhereColumnNotLikeString()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn NOT LIKE '%test%';";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("NOT LIKE"),
                        Right = new SqlStringNode("%test%")
                    }
                }
            );
        }

        [Test]
        public void Select_WhereExists()
        {
            const string s = "SELECT * FROM MyTable1 WHERE EXISTS (SELECT * FROM MyTable2);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable1"),
                    WhereClause = new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode("EXISTS"),
                        Right = new SqlParenthesisNode<SqlNode>(new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            },
                            FromClause = new SqlObjectIdentifierNode("MyTable2")
                        })
                    }
                }
            );
        }

        [Test]
        public void Select_WhereNotExists()
        {
            const string s = "SELECT * FROM MyTable1 WHERE NOT EXISTS (SELECT * FROM MyTable2);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable1"),
                    WhereClause = new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode("NOT"),
                        Right = new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("EXISTS"),
                            Right = new SqlParenthesisNode<SqlNode>(new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("MyTable2")
                            })
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereEqualsAllSelect()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn = ALL (SELECT 5);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("= ALL"),
                        Right = new SqlParenthesisNode<SqlNode>
                        {
                            Expression = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
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
            const string s = "SELECT * FROM MyTable WHERE MyColumn1 = 1 AND (MyColumn2 = 2 OR MyColumn3 = 3);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn1"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        },
                        Operator = new SqlOperatorNode("AND"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            },
                            Operator = new SqlOperatorNode("OR"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn3"),
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
            const string s = "SELECT * FROM MyTable WHERE MyColumn1 = 1 AND MyColumn2 = 2 OR MyColumn3 = 3;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("AND"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            }
                        },
                        Operator = new SqlOperatorNode("OR"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn3"),
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
            const string s = "SELECT * FROM MyTable WHERE (MyColumn1 = 1 AND MyColumn2 = 2) OR MyColumn3 = 3;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("AND"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            }
                        },
                        Operator = new SqlOperatorNode("OR"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn3"),
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
            const string s = "SELECT * FROM MyTable WHERE NOT MyColumn1 = 1 OR MyColumn2 = 2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("NOT"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            }
                        },
                        Operator = new SqlOperatorNode("OR"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn2"),
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
            const string s = "SELECT * FROM MyTable WHERE NOT (MyColumn1 = 1 OR MyColumn2 = 2);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode("NOT"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("OR"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn2"),
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