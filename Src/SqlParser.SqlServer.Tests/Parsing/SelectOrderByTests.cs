﻿using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class SelectOrderByTests
    {
        [Test]
        public void Select_OrderByColumnDesc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn DESC;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    OrderByClause = new SqlOrderByNode
                    {
                        Entries = new SqlListNode<SqlOrderByEntryNode>
                        {
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlIdentifierNode("MyColumn"),
                                Direction = "DESC"
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_OrderByNumberDesc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY 1 DESC;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    OrderByClause = new SqlOrderByNode
                    {
                        Entries = new SqlListNode<SqlOrderByEntryNode>
                        {
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlNumberNode(1),
                                Direction = "DESC"
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_OrderByColumnAsc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn ASC;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    OrderByClause = new SqlOrderByNode
                    {
                        Entries = new SqlListNode<SqlOrderByEntryNode>
                        {
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlIdentifierNode("MyColumn"),
                                Direction = "ASC"
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_OrderByColumnNone()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    OrderByClause = new SqlOrderByNode
                    {
                        Entries = new SqlListNode<SqlOrderByEntryNode>
                        {
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlIdentifierNode("MyColumn")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_OrderByColumnNoneOffsetNumberFetchNumber()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    OrderByClause = new SqlOrderByNode
                    {
                        Entries = new SqlListNode<SqlOrderByEntryNode>
                        {
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlIdentifierNode("MyColumn")
                            }
                        }
                    },
                    OffsetClause = new SqlNumberNode(5),
                    FetchClause = new SqlNumberNode(10)
                }
            );
        }

        [Test]
        public void Select_OrderByColumnAscDesc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn1 ASC, MyColumn2 DESC;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    OrderByClause = new SqlOrderByNode
                    {
                        Entries = new SqlListNode<SqlOrderByEntryNode>
                        {
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlIdentifierNode("MyColumn1"),
                                Direction = "ASC"
                            },
                            new SqlOrderByEntryNode
                            {
                                Source = new SqlIdentifierNode("MyColumn2"),
                                Direction = "DESC"
                            }
                        }
                    }
                }
            );
        }
    }
}