using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlStandard.Tests.Utility;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tokenizing;

namespace SqlParser.SqlStandard.Tests.Parsing
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
                                    new SqlOperatorNode("*")
                                },
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        }
                    },
                    Statement = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
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
                        Columns = new SqlListNode<ISqlNode>
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
            var result = target.Parse(s);
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
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1")
                        },
                        new SqlWithCteNode
                        {
                            Select = new SqlSelectNode
                            {
                                Columns = new SqlListNode<ISqlNode>
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
                        Columns = new SqlListNode<ISqlNode>
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
                            Columns = new SqlListNode<ISqlNode>
                            {
                                new SqlIdentifierNode("cola")
                            },
                            FromClause = new SqlObjectIdentifierNode("Cte1")
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
Cte1 AS (
    SELECT cola FROM MyTable
)
MERGE table1 AS TARGET
    USING MyTable AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN UPDATE SET TARGET.StatusCode = 'OK'
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
                                FromClause = new SqlObjectIdentifierNode("MyTable")
                            },
                            Name = new SqlIdentifierNode("Cte1")
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
                            Source = new SqlObjectIdentifierNode("MyTable"),
                            Alias = new SqlIdentifierNode("SOURCE")
                        },
                        MergeCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("TARGET"),
                                Identifier = new SqlIdentifierNode("Id")
                            },
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("SOURCE"),
                                Identifier = new SqlIdentifierNode("Id")
                            }
                        },
                        MatchClauses = new SqlListNode<SqlMergeMatchClauseNode>
                        {
                            new SqlMergeMatchClauseNode
                            {
                                Keyword = new SqlKeywordNode("WHEN MATCHED"),
                                Action = new SqlUpdateNode
                                {
                                    SetClause = new SqlListNode<SqlInfixOperationNode>
                                    {
                                        new SqlInfixOperationNode
                                        {
                                            Left = new SqlQualifiedIdentifierNode
                                            {
                                                Qualifier = new SqlIdentifierNode("TARGET"),
                                                Identifier = new SqlIdentifierNode("StatusCode")
                                            },
                                            Operator = new SqlOperatorNode("="),
                                            Right = new SqlStringNode("OK")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }
    }
}