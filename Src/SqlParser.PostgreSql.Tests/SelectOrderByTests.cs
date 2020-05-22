//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Tokenizing;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class SelectOrderByTests
//    {
//        [Test]
//        public void Select_OrderByColumnDesc()
//        {
//            const string s = "SELECT * FROM mytable ORDER BY mycolumn DESC;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    OrderByClause = new SqlOrderByNode
//                    {
//                        Entries = new SqlListNode<SqlOrderByEntryNode>
//                        {
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlIdentifierNode("mycolumn"),
//                                Direction = "DESC"
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_OrderByNumberDesc()
//        {
//            const string s = "SELECT * FROM mytable ORDER BY 1 DESC;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    OrderByClause = new SqlOrderByNode
//                    {
//                        Entries = new SqlListNode<SqlOrderByEntryNode>
//                        {
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlNumberNode(1),
//                                Direction = "DESC"
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_OrderByColumnAsc()
//        {
//            const string s = "SELECT * FROM mytable ORDER BY mycolumn ASC;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    OrderByClause = new SqlOrderByNode
//                    {
//                        Entries = new SqlListNode<SqlOrderByEntryNode>
//                        {
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlIdentifierNode("mycolumn"),
//                                Direction = "ASC"
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_OrderByColumnNone()
//        {
//            const string s = "SELECT * FROM mytable ORDER BY mycolumn;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    OrderByClause = new SqlOrderByNode
//                    {
//                        Entries = new SqlListNode<SqlOrderByEntryNode>
//                        {
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlIdentifierNode("mycolumn")
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_OrderByColumnNoneOffsetNumberFetchNumber()
//        {
//            const string s = "SELECT * FROM mytable ORDER BY mycolumn OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    OrderByClause = new SqlOrderByNode
//                    {
//                        Entries = new SqlListNode<SqlOrderByEntryNode>
//                        {
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlIdentifierNode("mycolumn")
//                            }
//                        }
//                    },
//                    OffsetClause = new SqlNumberNode(5),
//                    FetchClause = new SqlNumberNode(10)
//                }
//            );
//        }

//        [Test]
//        public void Select_OrderByColumnAscDesc()
//        {
//            const string s = "SELECT * FROM mytable ORDER BY mycolumn1 ASC, mycolumn2 DESC;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    OrderByClause = new SqlOrderByNode
//                    {
//                        Entries = new SqlListNode<SqlOrderByEntryNode>
//                        {
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlIdentifierNode("mycolumn1"),
//                                Direction = "ASC"
//                            },
//                            new SqlOrderByEntryNode
//                            {
//                                Source = new SqlIdentifierNode("mycolumn2"),
//                                Direction = "DESC"
//                            }
//                        }
//                    }
//                }
//            );
//        }
//    }
//}