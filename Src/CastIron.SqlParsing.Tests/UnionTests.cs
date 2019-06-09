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
        [TestCase("UNION")]
        [TestCase("UNION ALL")]
        [TestCase("EXCEPT")]
        [TestCase("INTERSECT")]
        public void Select_UnionOperatorSelect(string op)
        {
            string s = $"SELECT * FROM Table1 {op} SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInfixOperationNode
                {
                    Left = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlObjectIdentifierNode("Table1")
                        }
                    },
                    Operator = new SqlOperatorNode(op),
                    Right = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlObjectIdentifierNode("Table2")
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
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInfixOperationNode
                {
                    Left = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
                            {
                                new SqlOperatorNode("*")
                            }
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlObjectIdentifierNode("Table1")
                        }
                    },
                    Operator = new SqlOperatorNode("UNION"),
                    Right = new SqlInfixOperationNode
                    {
                        Left = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                Children = new List<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                }
                            },
                            FromClause = new SqlSelectFromClauseNode
                            {
                                Source = new SqlObjectIdentifierNode("Table2")
                            }
                        },
                        Operator = new SqlOperatorNode("UNION ALL"),
                        Right = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                Children = new List<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                }
                            },
                            FromClause = new SqlSelectFromClauseNode
                            {
                                Source = new SqlObjectIdentifierNode("Table3")
                            }
                        }
                    }
                }
            );
        }
    }
}