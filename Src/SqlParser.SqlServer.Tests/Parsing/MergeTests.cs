using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.SqlServer.Parsing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    public class MergeTests
    {
        [Test]
        public void Merge_WhenMatched()
        {
            const string s = @"
MERGE table1 AS TARGET
    USING table2 AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN MATCHED THEN UPDATE SET TARGET.StatusCode = 'OK'
;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation();//.And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlMergeNode
                {
                    Target = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table1"),
                        Alias = new SqlIdentifierNode("TARGET")
                    },
                    Source = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table2"),
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
            );
        }

        [Test]
        public void Merge_WhenNotMatched()
        {
            const string s = @"
MERGE table1 AS TARGET
    USING table2 AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN NOT MATCHED THEN INSERT (StatusCode) VALUES ('OK')
;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlMergeNode
                {
                    Target = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table1"),
                        Alias = new SqlIdentifierNode("TARGET")
                    },
                    Source = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table2"),
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
                            Keyword = new SqlKeywordNode("WHEN NOT MATCHED"),
                            Action = new SqlInsertNode
                            {
                                Columns = new SqlListNode<SqlIdentifierNode>
                                {
                                    new SqlIdentifierNode("StatusCode")
                                },
                                Source = new SqlValuesNode
                                {
                                    Values = new SqlListNode<SqlListNode<ISqlNode>>
                                    {
                                        new SqlListNode<ISqlNode>
                                        {
                                            new SqlStringNode("OK")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Merge_WhenNotMatchedByTarget()
        {
            const string s = @"
MERGE table1 AS TARGET
    USING table2 AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN NOT MATCHED BY TARGET THEN INSERT (StatusCode) VALUES ('OK')
;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlMergeNode
                {
                    Target = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table1"),
                        Alias = new SqlIdentifierNode("TARGET")
                    },
                    Source = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table2"),
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
                            Keyword = new SqlKeywordNode("WHEN NOT MATCHED BY TARGET"),
                            Action = new SqlInsertNode
                            {
                                Columns = new SqlListNode<SqlIdentifierNode>
                                {
                                    new SqlIdentifierNode("StatusCode")
                                },
                                Source = new SqlValuesNode
                                {
                                    Values = new SqlListNode<SqlListNode<ISqlNode>>
                                    {
                                        new SqlListNode<ISqlNode>
                                        {
                                            new SqlStringNode("OK")
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Merge_WhenNotMatchedBySource()
        {
            const string s = @"
MERGE table1 AS TARGET
    USING table2 AS SOURCE
    ON TARGET.Id = SOURCE.Id
    WHEN NOT MATCHED BY SOURCE THEN DELETE
;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlMergeNode
                {
                    Target = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table1"),
                        Alias = new SqlIdentifierNode("TARGET")
                    },
                    Source = new SqlAliasNode
                    {
                        Source = new SqlObjectIdentifierNode("table2"),
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
                            Keyword = new SqlKeywordNode("WHEN NOT MATCHED BY SOURCE"),
                            Action = new SqlKeywordNode("DELETE")
                        }
                    }
                }
            );
        }
    }
}
