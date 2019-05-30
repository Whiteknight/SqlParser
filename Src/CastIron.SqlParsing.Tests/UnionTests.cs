using System.Collections.Generic;
using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class UnionTests
    {
        [Test]
        public void Select_UnionSelect()
        {
            const string s = "SELECT * FROM Table1 UNION SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlUnionStatementNode
                {
                    First = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table1")
                        }
                    },
                    Operator = "UNION",
                    Second = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table2")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_UnionSelect2x()
        {
            const string s = "SELECT * FROM Table1 UNION SELECT * FROM Table2 UNION ALL SELECT * FROM Table3;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlUnionStatementNode
                {
                    First = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table1")
                        }
                    },
                    Operator = "UNION",
                    Second = new SqlUnionStatementNode
                    {
                        First = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                Children = new List<SqlNode>
                                {
                                    new SqlStarNode()
                                }
                            },
                            FromClause = new SqlSelectFromClauseNode
                            {
                                Source = new SqlIdentifierNode("Table2")
                            }
                        },
                        Operator = "UNION ALL",
                        Second = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                Children = new List<SqlNode>
                                {
                                    new SqlStarNode()
                                }
                            },
                            FromClause = new SqlSelectFromClauseNode
                            {
                                Source = new SqlIdentifierNode("Table3")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_UnionAllSelect()
        {
            const string s = "SELECT * FROM Table1 UNION ALL SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlUnionStatementNode
                {
                    First = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table1")
                        }
                    },
                    Operator = "UNION ALL",
                    Second = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table2")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ExceptSelect()
        {
            const string s = "SELECT * FROM Table1 EXCEPT SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlUnionStatementNode
                {
                    First = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table1")
                        }
                    },
                    Operator = "EXCEPT",
                    Second = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table2")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_IntersectSelect()
        {
            const string s = "SELECT * FROM Table1 INTERSECT SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlUnionStatementNode
                {
                    First = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table1")
                        }
                    },
                    Operator = "INTERSECT",
                    Second = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlStarNode()
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlIdentifierNode("Table2")
                        }
                    }
                }
            );
        }
    }
}