using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests.SqlServer
{
    [TestFixture]
    public class IfTests
    {
        [Test]
        public void If_ConditionThen()
        {
            const string s = @"IF 5 = 6 SELECT 'TEST';";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlIfNode
                {
                    Condition = new SqlInfixOperationNode
                    {
                        Left = new SqlNumberNode(5),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(6)
                    },
                    Then = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void If_ParenConditionThen()
        {
            const string s = @"IF (5 = 6) SELECT 'TEST';";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlIfNode
                {
                    Condition = new SqlInfixOperationNode
                    {
                        Left = new SqlNumberNode(5),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(6)
                    },
                    Then = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void If_ParenConditionThenBlock()
        {
            const string s = @"
IF (5 = 6) 
BEGIN 
    SELECT 'TEST1';
    SELECT 'TEST2';
END";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            var thenStatementList = new SqlStatementListNode
            {
                UseBeginEnd = true
            };
            thenStatementList.Add( new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
                {
                    Children = new List<ISqlNode>
                    {
                        new SqlStringNode("TEST1")
                    }
                }
            });
            thenStatementList.Add(new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
                {
                    Children = new List<ISqlNode>
                    {
                        new SqlStringNode("TEST2")
                    }
                }
            });

            var expectedAst = new SqlIfNode
            {
                Condition = new SqlInfixOperationNode
                {
                    Left = new SqlNumberNode(5),
                    Operator = new SqlOperatorNode("="),
                    Right = new SqlNumberNode(6)
                },
                Then = thenStatementList
            };

            result.Statements.First().Should().MatchAst(expectedAst);
        }

        [Test]
        public void If_ConditionThenElse()
        {
            const string s = @"IF 5 = 6 SELECT 'TEST1'; ELSE SELECT 'TEST2';";
            
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlIfNode
                {
                    Condition = new SqlInfixOperationNode
                    {
                        Left = new SqlNumberNode(5),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(6)
                    },
                    Then = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST1")
                            }
                        }
                    },
                    Else = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST2")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void If_ThenBlockElseBlock()
        {
            const string s = @"
IF (5 = 6) 
BEGIN 
    SELECT 'TEST1';
    SELECT 'TEST2';
END
ELSE
BEGIN
    SELECT 'TEST3';
    SELECT 'TEST4';
END";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            var thenStatementList = new SqlStatementListNode
            {
                UseBeginEnd = true
            };
            thenStatementList.Add(new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
                {
                    Children = new List<ISqlNode>
                    {
                        new SqlStringNode("TEST1")
                    }
                }
            });
            thenStatementList.Add(new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
                {
                    Children = new List<ISqlNode>
                    {
                        new SqlStringNode("TEST2")
                    }
                }
            });

            var elseStatementList = new SqlStatementListNode
            {
                UseBeginEnd = true
            };
            elseStatementList.Add(new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
                {
                    Children = new List<ISqlNode>
                    {
                        new SqlStringNode("TEST3")
                    }
                }
            });
            elseStatementList.Add(new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
                {
                    Children = new List<ISqlNode>
                    {
                        new SqlStringNode("TEST4")
                    }
                }
            });

            var expectedAst = new SqlIfNode
            {
                Condition = new SqlInfixOperationNode
                {
                    Left = new SqlNumberNode(5),
                    Operator = new SqlOperatorNode("="),
                    Right = new SqlNumberNode(6)
                },
                Then = thenStatementList,
                Else = elseStatementList
            };

            result.Statements.First().Should().MatchAst(expectedAst);
        }
    }
}
