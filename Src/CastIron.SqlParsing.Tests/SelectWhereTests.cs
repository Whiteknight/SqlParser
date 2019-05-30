using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectWhereTests
    {
        [Test]
        public void Select_WhereColumnEqualsNumber()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn = 1;";
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
                        Source = new SqlIdentifierNode("MyTable")
                    },
                    WhereClause = new SqlSelectWhereClauseNode
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
        public void Select_WhereColumnEqualsExpression()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn1 = MyColumn2 + 1;";
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
                        Source = new SqlIdentifierNode("MyTable")
                    },
                    WhereClause = new SqlSelectWhereClauseNode
                    {
                        SearchCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn1"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlInfixOperationNode
                            {
                                Left = new SqlIdentifierNode("MyColumn2"),
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlNumberNode(1)
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_WhereAnd()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn1 = 1 AND MyColumn2 = 2;";
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
                        Source = new SqlIdentifierNode("MyTable")
                    },
                    WhereClause = new SqlSelectWhereClauseNode
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
        public void Select_WhereColumnBetween()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn BETWEEN 1 AND 2;";
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
                        Source = new SqlIdentifierNode("MyTable")
                    },
                    WhereClause = new SqlSelectWhereClauseNode
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
        public void Select_WhereColumnInNumberList()
        {
            const string s = "SELECT * FROM MyTable WHERE MyColumn IN (1, 2, 3);";
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
                        Source = new SqlIdentifierNode("MyTable")
                    },
                    WhereClause = new SqlSelectWhereClauseNode
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