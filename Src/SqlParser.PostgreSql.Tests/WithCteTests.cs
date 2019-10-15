using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class WithCteTests
    {
        [Test]
        public void With_Select()
        {
            const string s = @"
WITH 
cte1 AS (
    SELECT * FROM mytable
)
SELECT * FROM cte1;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
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
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("mytable")
                            },
                            Name = new SqlIdentifierNode("cte1")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause =  new SqlObjectIdentifierNode("cte1")
                    }
                }
            );
        }

        [Test]
        public void With_RecursiveSelect()
        {
            const string s = @"
WITH 
RECURSIVE cte1 AS (
    SELECT * FROM mytable
    UNION ALL
    SELECT * from cte1
)
SELECT * FROM cte1;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlWithNode
                {
                    Ctes = new SqlListNode<SqlWithCteNode>
                    {
                        new SqlWithCteNode
                        {
                            Recursive = true,
                            Select = new SqlInfixOperationNode
                            {
                                Left = new SqlSelectNode
                                {
                                    Columns = new SqlListNode<ISqlNode>
                                    {
                                        new SqlOperatorNode("*")
                                    },
                                    FromClause = new SqlObjectIdentifierNode("mytable")
                                },
                                Operator = new SqlOperatorNode("UNION ALL"),
                                Right = new SqlSelectNode
                                {
                                    Columns = new SqlListNode<ISqlNode>
                                    {
                                        new SqlOperatorNode("*")
                                    },
                                    FromClause = new SqlObjectIdentifierNode("cte1")
                                }
                            },
                            Name = new SqlIdentifierNode("cte1")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause = new SqlObjectIdentifierNode("cte1")
                    }
                }
            );
        }

        [Test]
        public void With_ColumnNamesSelect()
        {
            const string s = @"
WITH 
cte1(columna) AS (
    SELECT * FROM mytable
)
SELECT * FROM cte1;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
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
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("mytable")
                            },
                            Name = new SqlIdentifierNode("cte1"),
                            ColumnNames = new SqlListNode<SqlIdentifierNode>
                            {
                                new SqlIdentifierNode("columna")
                            }
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause = new SqlObjectIdentifierNode("cte1")
                    }
                }
            );
        }

        [Test]
        public void With_SelectTwoCtes()
        {
            const string s = @"
WITH 
cte1 AS (
    SELECT * FROM mytable
),
cte2 AS (
    SELECT * FROM cte1
)
SELECT * FROM cte2;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
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
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("mytable")
                            },
                            Name = new SqlIdentifierNode("cte1")
                        },
                        new SqlWithCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("cte1")
                            },
                            Name = new SqlIdentifierNode("cte2")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            new SqlOperatorNode("*")
                        },
                        FromClause = new SqlObjectIdentifierNode("cte2")
                    }
                }
            );
        }

        [Test]
        public void With_InsertIntoSelect()
        {
            const string s = @"
WITH 
cte1 AS (
    SELECT cola FROM mytable
)
INSERT INTO mytable(columna) 
    SELECT cola FROM cte1;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
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
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlIdentifierNode("cola")
                                },
                                FromClause = new SqlObjectIdentifierNode("mytable")
                            },
                            Name = new SqlIdentifierNode("cte1")
                        }
                    },
                    Statement = new SqlInsertNode
                    {
                        Table = new SqlObjectIdentifierNode("mytable"),
                        Columns = new SqlListNode<SqlIdentifierNode>
                        {
                            new SqlIdentifierNode("columna")
                        },
                        Source = new SqlSelectNode
                        {
                            Columns = new SqlListNode<ISqlNode>
                            {
                                new SqlIdentifierNode("cola")
                            },
                            FromClause = new SqlObjectIdentifierNode("cte1")
                        }
                    }
                }
            );
        }

        [Test]
        public void With_Merge()
        {
            const string s = @"
WITH 
cte1 AS (
    SELECT cola FROM mytable
)
MERGE table1 AS TARGET
    USING mytable AS SOURCE
    ON TARGET.id = SOURCE.id
    WHEN MATCHED THEN UPDATE SET TARGET.statuscode = 'OK'
;";
            var target = new Parser();
            var result = target.Parse(s);
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
                                Columns = new SqlListNode<ISqlNode>
                                {
                                    new SqlIdentifierNode("cola")
                                },
                                FromClause = new SqlObjectIdentifierNode("mytable")
                            },
                            Name = new SqlIdentifierNode("cte1")
                        }
                    },
                    Statement = new SqlMergeNode
                    {
                        Target = new SqlAliasNode
                        {
                            Source = new SqlObjectIdentifierNode("table1"),
                            Alias = new SqlIdentifierNode("TARGET")
                        },
                        Source = new SqlAliasNode
                        {
                            Source = new SqlObjectIdentifierNode("mytable"),
                            Alias = new SqlIdentifierNode("SOURCE")
                        },
                        MergeCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("TARGET"),
                                Identifier = new SqlIdentifierNode("id")
                            },
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("SOURCE"),
                                Identifier = new SqlIdentifierNode("id")
                            }
                        },
                        Matched = new SqlMergeUpdateNode
                        {
                            SetClause = new SqlListNode<SqlInfixOperationNode>
                            {
                                new SqlInfixOperationNode
                                {
                                    Left = new SqlQualifiedIdentifierNode
                                    {
                                        Qualifier = new SqlIdentifierNode("TARGET"),
                                        Identifier = new SqlIdentifierNode("statuscode")
                                    },
                                    Operator = new SqlOperatorNode("="),
                                    Right = new SqlStringNode("OK")
                                }
                            }
                        }
                    }
                }
            );
        }
    }
}