using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectExpressionTests
    { 
        [Test]
        public void Select_ArithmeticExpression1()
        {
            const string s = "SELECT 1 + 2 * 3 AS ColumnA";
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
                            Source = new SqlInfixOperationNode
                            {
                                Left = new SqlNumberNode(1),
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlInfixOperationNode
                                {
                                    Left = new SqlNumberNode(2),
                                    Operator = new SqlOperatorNode("*"),
                                    Right = new SqlNumberNode(3)
                                }
                            }
                            
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ArithmeticExpression()
        {
            const string s = "SELECT 1 * 2 + 3 AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            var output = result.ToString();
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("ColumnA"),
                            Source = new SqlInfixOperationNode
                            {
                                Left = new SqlInfixOperationNode
                                {
                                    Left = new SqlNumberNode(1),
                                    Operator = new SqlOperatorNode("*"),
                                    Right = new SqlNumberNode(2)
                                },
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlNumberNode(3)
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ArithmeticExpression2()
        {
            const string s = "SELECT 1 + 2 * -3 AS ColumnA";
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
                            Source = new SqlInfixOperationNode
                            {
                                Left = new SqlNumberNode(1),
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlInfixOperationNode
                                {
                                    Left = new SqlNumberNode(2),
                                    Operator = new SqlOperatorNode("*"),
                                    Right = new SqlPrefixOperationNode {
                                        Operator = new SqlOperatorNode("-"),
                                        Right = new SqlNumberNode(3)
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ArithmeticExpressionParens()
        {
            const string s = "SELECT 1 * (2 + 3)";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            var output = result.ToString();
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlInfixOperationNode
                        {
                            Left =new SqlNumberNode(1),
                            Operator = new SqlOperatorNode("*"),
                            Right =  new SqlInfixOperationNode
                            {
                                Left = new SqlNumberNode(2),
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlNumberNode(3)
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ColumnArithmeticExpression()
        {
            const string s = "SELECT ColumnA + ColumnB * ColumnC AS ColumnD";
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
                            Alias = new SqlIdentifierNode("ColumnD"),
                            Source = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("ColumnA"),
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlInfixOperationNode
                                {
                                    Left = new SqlIdentifierNode("ColumnB"),
                                    Operator = new SqlOperatorNode("*"),
                                    Right = new SqlIdentifierNode("ColumnC")
                                }
                            }

                        }
                    }
                }
            );
        }
    }
}