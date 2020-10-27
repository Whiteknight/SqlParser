using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlStandard.Tests.Utility;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tokenizing;

namespace SqlParser.SqlStandard.Tests.Parsing
{
    [TestFixture]
    public class SelectCaseTests
    { 

        [Test]
        public void Select_CaseExprWhenThenElseEnd()
        {
            const string s = "SELECT CASE 5 WHEN 6 THEN 'A' ELSE 'B' END;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();
            var output = result.ToString();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
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
        public void Select_CaseNoExprWhenThenElseEnd()
        {
            const string s = "SELECT CASE WHEN 6=5 THEN 'A' ELSE 'B' END;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();
            var output = result.ToString();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlCaseNode
                        {
                            WhenExpressions = new List<SqlCaseWhenNode>
                            {
                                new SqlCaseWhenNode
                                {
                                    Condition = new SqlInfixOperationNode {
                                        Left = new SqlNumberNode(6),
                                        Operator = new SqlOperatorNode("="),
                                        Right = new SqlNumberNode(5)
                                    },
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
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
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