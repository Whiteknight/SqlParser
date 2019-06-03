using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectHavingTests
    {
        [Test]
        public void Select_HavingColumnEqualsNumber()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn = 1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    HavingClause = new SqlSelectHavingClauseNode
                    {
                        SearchCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_HavingAnd()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn1 = 1 AND MyColumn2 = 2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    HavingClause = new SqlSelectHavingClauseNode
                    {
                        SearchCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn1"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(1)
                            },
                            Operator = new SqlOperatorNode("AND"),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn2"),
                                Operator = new SqlOperatorNode("="),
                                Right = new SqlNumberNode(2)
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_HavingColumnBetween()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn BETWEEN 1 AND 2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    HavingClause = new SqlSelectHavingClauseNode
                    {
                        SearchCondition = new SqlBetweenOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn"),
                            Low = new SqlNumberNode(1),
                            High = new SqlNumberNode(2)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_HavingColumnInNumberList()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn IN (1, 2, 3);";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            var output = result.ToString();
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    },
                    HavingClause = new SqlSelectHavingClauseNode
                    {
                        SearchCondition = new SqlInNode
                        {
                            Search = new SqlIdentifierNode("MyColumn"),
                            Items = new SqlListNode<SqlNode>
                            {
                                new SqlNumberNode(1),
                                new SqlNumberNode(2),
                                new SqlNumberNode(3)
                            }
                        }
                    }
                }
            );
        }
    }
}