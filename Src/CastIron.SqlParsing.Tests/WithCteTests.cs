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
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlWithNode
                {
                    Ctes = new SqlListNode<SqlWithCteNode>
                    {
                        new SqlWithCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause =  new SqlObjectIdentifierNode("Cte1")
                    }
                }
            );
        }

        [Test]
        public void With_ColumnNamesSelect()
        {
            const string s = @"
WITH 
Cte1(ColumnA) AS (
    SELECT * FROM MyTable
)
SELECT * FROM Cte1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlWithNode
                {
                    Ctes = new SqlListNode<SqlWithCteNode>
                    {
                        new SqlWithCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1"),
                            ColumnNames = new SqlListNode<SqlIdentifierNode>
                            {
                                new SqlIdentifierNode("ColumnA")
                            }
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause = new SqlObjectIdentifierNode("Cte1")
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
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlWithNode
                {
                    Ctes = new SqlListNode<SqlWithCteNode>
                    {
                        new SqlWithCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        },
                        new SqlWithCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<SqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("Cte1")
                            },
                            Name = new SqlIdentifierNode("Cte2")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<SqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause = new SqlObjectIdentifierNode("Cte2")
                    }
                }
            );
        }
    }
}