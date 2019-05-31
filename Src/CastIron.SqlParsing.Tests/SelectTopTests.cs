﻿using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectTopTests
    {
        [Test]
        public void Select_TopNumber()
        {
            const string s = "SELECT TOP 10 * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlNumberNode(10)
                    }
                }
            );
        }

        [Test]
        public void Select_TopVariable()
        {
            const string s = "SELECT TOP @limit * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlVariableNode("@limit")
                    }
                }
            );
        }

        [Test]
        public void Select_TopNumberParens()
        {
            const string s = "SELECT TOP (10) * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlNumberNode(10)
                    }
                }
            );
        }

        [Test]
        public void Select_TopVariableParens()
        {
            const string s = "SELECT TOP (@limit) * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlVariableNode("@limit")
                    }
                }
            );
        }

        [Test]
        public void Select_TopNumberParensPercent()
        {
            const string s = "SELECT TOP (10) PERCENT * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlNumberNode(10),
                        Percent = true
                    }
                }
            );
        }

        [Test]
        public void Select_TopNumberParensWithTies()
        {
            const string s = "SELECT TOP (10) WITH TIES * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlNumberNode(10),
                        WithTies = true
                    }
                }
            );
        }

        [Test]
        public void Select_TopNumberParensPercentWithTies()
        {
            const string s = "SELECT TOP (10) PERCENT WITH TIES * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    TopClause = new SqlSelectTopNode
                    {
                        Value = new SqlNumberNode(10),
                        Percent = true,
                        WithTies = true
                    }
                }
            );
        }
    }
}