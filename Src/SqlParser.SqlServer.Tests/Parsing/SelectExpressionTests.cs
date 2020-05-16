using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.SqlStandard;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class SelectExpressionTests
    { 
        [Test]
        public void Select_ArithmeticExpression1()
        {
            const string s = "SELECT 1 + 2 * 3 AS ColumnA";
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
            var target = new Parser();
            var result = target.Parse(s);
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
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
                            Source = new SqlInfixOperationNode
                            {
                                Left = new SqlNumberNode(1),
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlInfixOperationNode
                                {
                                    Left = new SqlNumberNode(2),
                                    Operator = new SqlOperatorNode("*"),
                                    Right = new SqlNumberNode(-3)
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
            var target = new Parser();
            var result = target.Parse(s);
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
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

        [Test]
        public void Select_SelectExpression()
        {
            const string s = "SELECT (SELECT 5)";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation();//.And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlParenthesisNode<ISqlNode>
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
        public void Select_CastVarcharMax()
        {
            const string s = "SELECT CAST(5 AS VARCHAR(MAX))";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlCastNode
                        {
                            Expression = new SqlNumberNode(5),
                            DataType = new SqlDataTypeNode
                            {
                                DataType = new SqlKeywordNode("VARCHAR"),
                                Size = new SqlKeywordNode("MAX")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_CastVarcharNumber()
        {
            const string s = "SELECT CAST(5 AS VARCHAR(3))";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlCastNode
                        {
                            Expression = new SqlNumberNode(5),
                            DataType = new SqlDataTypeNode
                            {
                                DataType = new SqlKeywordNode("VARCHAR"),
                                Size = new SqlListNode<SqlNumberNode> {
                                    new SqlNumberNode(3),
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_CastNumericPS()
        {
            const string s = "SELECT CAST(5 AS NUMERIC(10, 5))";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlCastNode
                        {
                            Expression = new SqlNumberNode(5),
                            DataType = new SqlDataTypeNode
                            {
                                DataType = new SqlKeywordNode("NUMERIC"),
                                Size = new SqlListNode<SqlNumberNode> {
                                    new SqlNumberNode(10),
                                    new SqlNumberNode(5)
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_CastNullAsInt()
        {
            const string s = "SELECT CAST(NULL AS int)";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlCastNode
                        {
                            Expression = new SqlNullNode(),
                            DataType = new SqlDataTypeNode
                            {
                                DataType = new SqlKeywordNode("INT")
                            }
                        }
                    }
                }
            );
        }
    }
}