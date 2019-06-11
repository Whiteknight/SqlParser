﻿using System.Collections.Generic;
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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
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
        public void Select_NegativeIntegerConstant()
        {
            const string s = "SELECT -10";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("-"),
                            Right = new SqlNumberNode(10)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_PrefixesIntegerConstant()
        {
            const string s = "SELECT ~+-10";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("~"),
                            Right = new SqlPrefixOperationNode
                            {
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlPrefixOperationNode
                                {
                                    Operator = new SqlOperatorNode("-"),
                                    Right = new SqlNumberNode(10)
                                }
                            }
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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
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
        public void Select_NegativeVariable()
        {
            const string s = "SELECT -@value";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("-"),
                            Right = new SqlVariableNode("@value")
                        }
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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
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
        public void Select_FunctionCall()
        {
            const string s = "SELECT GETUTCDATE() AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("GETUTCDATE")
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_FunctionCallArgument()
        {
            const string s = "SELECT ABS(1) AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("ABS"),
                                Arguments = new SqlListNode<SqlNode>
                                {
                                    new SqlNumberNode(1)
                                }
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_FunctionCall2Arguments()
        {
            const string s = "SELECT COALESCE(NULL, 0) AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("COALESCE"),
                                Arguments = new SqlListNode<SqlNode>
                                {
                                    new SqlNullNode(),
                                    new SqlNumberNode(0)
                                }
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_Null()
        {
            const string s = "SELECT NULL AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlNullNode(),
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }
    }
}