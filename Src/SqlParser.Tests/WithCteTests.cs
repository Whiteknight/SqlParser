using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests
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
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
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
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
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
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
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

        [Test]
        public void With_InsertIntoSelect()
        {
            const string s = @"
WITH 
Cte1 AS (
    SELECT cola FROM MyTable
)
INSERT INTO MyTable(ColumnA) 
    SELECT cola FROM Cte1;";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
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
                                    new SqlIdentifierNode("cola")
                                },
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        }
                    },
                    Statement = new SqlInsertNode
                    {
                        Table = new SqlObjectIdentifierNode("MyTable"),
                        Columns = new SqlListNode<SqlIdentifierNode>
                        {
                            new SqlIdentifierNode("ColumnA")
                        },
                        Source = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
                            {
                                new SqlIdentifierNode("cola")
                            },
                            FromClause = new SqlObjectIdentifierNode("Cte1")
                        }
                    }
                }
            );
        }
    }
}