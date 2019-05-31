using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class WithCteTests
    {
        [Test]
        public void With_Select()
        {
            const string s = @"
WITH 
Cte1 AS (
    SELECT * FROM MyTable
)
SELECT * FROM Cte1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlWithNode
                {
                    Ctes = new SqlListNode<SqlCteNode>
                    {
                        new SqlCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlStarNode()
                                },
                                FromClause = new SqlSelectFromClauseNode
                                {
                                    Source = new SqlObjectIdentifierNode("MyTable")
                                }
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            new SqlStarNode()
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlObjectIdentifierNode("Cte1")
                        }
                    }
                }
            );
        }

        [Test]
        public void With_SelectTwoCtes()
        {
            const string s = @"
WITH 
Cte1 AS (
    SELECT * FROM MyTable
),
Cte2 AS (
    SELECT * FROM Cte1
)
SELECT * FROM Cte2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlWithNode
                {
                    Ctes = new SqlListNode<SqlCteNode>
                    {
                        new SqlCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlStarNode()
                                },
                                FromClause = new SqlSelectFromClauseNode
                                {
                                    Source = new SqlObjectIdentifierNode("MyTable")
                                }
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        },
                        new SqlCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlStarNode()
                                },
                                FromClause = new SqlSelectFromClauseNode
                                {
                                    Source = new SqlObjectIdentifierNode("Cte1")
                                }
                            },
                            Name = new SqlIdentifierNode("Cte2")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            new SqlStarNode()
                        },
                        FromClause = new SqlSelectFromClauseNode
                        {
                            Source = new SqlObjectIdentifierNode("Cte2")
                        }
                    }
                }
            );
        }
    }
}