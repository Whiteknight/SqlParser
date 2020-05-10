//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class MergeTests
//    {
//        [Test]
//        public void Merge_WhenMatched()
//        {
//            const string s = @"
//MERGE table1 AS TARGET
//    USING table2 AS SOURCE
//    ON TARGET.id = SOURCE.id
//    WHEN MATCHED THEN UPDATE SET TARGET.statuscode = 'OK'
//;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlMergeNode
//                {
//                    Target = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table1"),
//                        Alias = new SqlIdentifierNode("TARGET")
//                    },
//                    Source = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table2"),
//                        Alias = new SqlIdentifierNode("SOURCE")
//                    },
//                    MergeCondition = new SqlInfixOperationNode
//                    {
//                        Left = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("TARGET"),
//                            Identifier = new SqlIdentifierNode("id")
//                        },
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("SOURCE"),
//                            Identifier = new SqlIdentifierNode("id")
//                        }
//                    },
//                    Matched = new SqlUpdateNode
//                    {
//                        SetClause = new SqlListNode<SqlInfixOperationNode>
//                        {
//                            new SqlInfixOperationNode
//                            {
//                                Left = new SqlQualifiedIdentifierNode
//                                {
//                                    Qualifier = new SqlIdentifierNode("TARGET"),
//                                    Identifier = new SqlIdentifierNode("statuscode")
//                                },
//                                Operator = new SqlOperatorNode("="),
//                                Right = new SqlStringNode("OK")
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Merge_WhenNotMatched()
//        {
//            const string s = @"
//MERGE table1 AS TARGET
//    USING table2 AS SOURCE
//    ON TARGET.id = SOURCE.id
//    WHEN NOT MATCHED THEN INSERT (statuscode) VALUES ('OK')
//;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlMergeNode
//                {
//                    Target = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table1"),
//                        Alias = new SqlIdentifierNode("TARGET")
//                    },
//                    Source = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table2"),
//                        Alias = new SqlIdentifierNode("SOURCE")
//                    },
//                    MergeCondition = new SqlInfixOperationNode
//                    {
//                        Left = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("TARGET"),
//                            Identifier = new SqlIdentifierNode("id")
//                        },
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("SOURCE"),
//                            Identifier = new SqlIdentifierNode("id")
//                        }
//                    },
//                    NotMatchedByTarget = new SqlInsertNode
//                    {
//                        Columns = new SqlListNode<SqlIdentifierNode>
//                        {
//                            new SqlIdentifierNode("statuscode")
//                        },
//                        Source = new SqlValuesNode
//                        {
//                            Values = new SqlListNode<SqlListNode<ISqlNode>>
//                            {
//                                new SqlListNode<ISqlNode>
//                                {
//                                    new SqlStringNode("OK")
//                                }
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Merge_WhenNotMatchedByTarget()
//        {
//            const string s = @"
//MERGE table1 AS TARGET
//    USING table2 AS SOURCE
//    ON TARGET.id = SOURCE.id
//    WHEN NOT MATCHED BY TARGET THEN INSERT (statuscode) VALUES ('OK')
//;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlMergeNode
//                {
//                    Target = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table1"),
//                        Alias = new SqlIdentifierNode("TARGET")
//                    },
//                    Source = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table2"),
//                        Alias = new SqlIdentifierNode("SOURCE")
//                    },
//                    MergeCondition = new SqlInfixOperationNode
//                    {
//                        Left = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("TARGET"),
//                            Identifier = new SqlIdentifierNode("id")
//                        },
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("SOURCE"),
//                            Identifier = new SqlIdentifierNode("id")
//                        }
//                    },
//                    NotMatchedByTarget = new SqlInsertNode
//                    {
//                        Columns = new SqlListNode<SqlIdentifierNode>
//                        {
//                            new SqlIdentifierNode("statuscode")
//                        },
//                        Source = new SqlValuesNode
//                        {
//                            Values = new SqlListNode<SqlListNode<ISqlNode>>
//                            {
//                                new SqlListNode<ISqlNode>
//                                {
//                                    new SqlStringNode("OK")
//                                }
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Merge_WhenNotMatchedBySource()
//        {
//            const string s = @"
//MERGE table1 AS TARGET
//    USING table2 AS SOURCE
//    ON TARGET.id = SOURCE.id
//    WHEN NOT MATCHED BY SOURCE THEN DELETE
//;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlMergeNode
//                {
//                    Target = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table1"),
//                        Alias = new SqlIdentifierNode("TARGET")
//                    },
//                    Source = new SqlAliasNode
//                    {
//                        Source = new SqlObjectIdentifierNode("table2"),
//                        Alias = new SqlIdentifierNode("SOURCE")
//                    },
//                    MergeCondition = new SqlInfixOperationNode
//                    {
//                        Left = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("TARGET"),
//                            Identifier = new SqlIdentifierNode("id")
//                        },
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlQualifiedIdentifierNode
//                        {
//                            Qualifier = new SqlIdentifierNode("SOURCE"),
//                            Identifier = new SqlIdentifierNode("id")
//                        }
//                    },
//                    NotMatchedBySource = new SqlKeywordNode("DELETE")
//                }
//            );
//        }
//    }
//}
