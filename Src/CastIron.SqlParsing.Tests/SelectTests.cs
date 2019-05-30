using System.Collections.Generic;
using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectTests
    {
        [Test]
        public void Select_StringConstant()
        {
            const string s = "SELECT 'TEST'";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlStringNode("TEST")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_StringConstantAlias()
        {
            const string s = "SELECT 'TEST' AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlAliasNode
                            {
                                Source = new SqlStringNode("TEST"),
                                Alias = new SqlIdentifierNode("ColumnA")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_IntegerConstant()
        {
            const string s = "SELECT 10";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlNumberNode(10)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_DecimalConstant()
        {
            const string s = "SELECT 10.123";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlNumberNode(10.123M)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_Variable()
        {
            const string s = "SELECT @value";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlVariableNode("@value")
                    }
                }
            );
        }

        [Test]
        public void Select_VariableAlias()
        {
            const string s = "SELECT @value AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlVariableNode("@value"),
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ArithmeticExpression()
        {
            const string s = "SELECT 1 + 2 * 3 AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
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
        public void Select_ColumnArithmeticExpression()
        {
            const string s = "SELECT ColumnA + ColumnB * ColumnC AS ColumnD";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            (result as SqlStatementListNode)?.Statements.First().Should().MatchAst(
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