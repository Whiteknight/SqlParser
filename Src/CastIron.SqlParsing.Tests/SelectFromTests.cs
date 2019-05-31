using System.Collections.Generic;
using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectFromTests
    {
        [Test]
        public void Select_StarFromTable()
        {
            const string s = "SELECT * FROM MyTable;";
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
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_StarFromSchemaTable()
        {
            const string s = "SELECT * FROM dbo.MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlStarNode()
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("dbo", "MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_SingleLineComments()
        {
            const string s = @"
            -- Before
            SELECT 
                -- FirstColumn,
                * 
                FROM 
                    MyTable
                    -- INNER JOIN SomeOtherTable ON MyTableId = SomeOtherTableId;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlStarNode()
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_MultiLineComments()
        {
            const string s = @"
            /*
             BEFORE
            */
            SELECT 
                /* FirstColumn, */  * 
                FROM 
                    MyTable /* INNER JOIN SomeOtherTable ON MyTableId = SomeOtherTableId */;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlStarNode()
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_DistinctStarFromTable()
        {
            const string s = "SELECT DISTINCT * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Modifier = "DISTINCT",
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlStarNode()
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_StarFromTableVariable()
        {
            const string s = "SELECT * FROM @tableVar;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlStarNode()
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlVariableNode("@tableVar")
                    }
                }
            );
        }

        [Test]
        public void Select_TableAlias()
        {
            const string s = "SELECT t1.* FROM MyTable AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t1"),
                                Identifier = new SqlStarNode()
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t1"),
                            Source = new SqlObjectIdentifierNode("MyTable")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_TableAliasColumn()
        {
            const string s = "SELECT t1.MyColumn FROM MyTable AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t1"),
                                Identifier = new SqlIdentifierNode("MyColumn")
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t1"),
                            Source = new SqlObjectIdentifierNode("MyTable")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_TableAliasBracketed()
        {
            const string s = "SELECT [t1].* FROM [MyTable] AS [t1];";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t1"),
                                Identifier = new SqlStarNode()
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t1"),
                            Source = new SqlObjectIdentifierNode("MyTable")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_TableVariableAlias()
        {
            const string s = "SELECT t1.* FROM @myTable AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t1"),
                                Identifier = new SqlStarNode()
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t1"),
                            Source = new SqlVariableNode("@myTable")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_TableExpression()
        {
            const string s = @"
                SELECT 
                    t1.* 
                    FROM 
                        (SELECT * FROM MyTable) AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t1"),
                                Identifier = new SqlStarNode()
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlAliasNode
                        {
                            Source = new SqlSelectSubexpressionNode
                            {
                                Select = new SqlSelectNode
                                {
                                    Columns = new SqlListNode<SqlNode>
                                    {
                                        Children = new List<SqlNode>
                                        {
                                            new SqlStarNode()
                                        }
                                    },
                                    FromClause = new SqlSelectFromClauseNode
                                    {
                                        Source = new SqlObjectIdentifierNode("MyTable")
                                    }
                                }
                            },
                            Alias = new SqlIdentifierNode("t1")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_ColumnsFromTable()
        {
            const string s = "SELECT ColumnA, ColumnB FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlIdentifierNode("ColumnA"),
                            new SqlIdentifierNode("ColumnB")
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_IdentityRowGuidFromTable()
        {
            const string s = "SELECT $IDENTITY, $ROWGUID FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlIdentifierNode("$IDENTITY"),
                            new SqlIdentifierNode("$ROWGUID")
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_BracketedColumnsFromTable()
        {
            const string s = "SELECT [ColumnA], [ColumnB] FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlIdentifierNode("ColumnA"),
                            new SqlIdentifierNode("ColumnB")
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_BracketedKeywordColumnsFromTable()
        {
            const string s = "SELECT [SELECT], [FROM] FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlIdentifierNode("SELECT"),
                            new SqlIdentifierNode("FROM")
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_ColumnAliasFromTable()
        {
            const string s = "SELECT ColumnA AS ColumnB FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlAliasNode
                            {
                                Source = new SqlIdentifierNode("ColumnA"),
                                Alias = new SqlIdentifierNode("ColumnB")
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_ColumnAliasBracketedFromTable()
        {
            const string s = "SELECT [ColumnA] AS [ColumnB] FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlAliasNode
                            {
                                Source = new SqlIdentifierNode("ColumnA"),
                                Alias = new SqlIdentifierNode("ColumnB")
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Select_VariableAssign()
        {
            const string s = "SELECT @value = ColumnA FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        Children = new List<SqlNode>
                        {
                            new SqlAssignVariableNode
                            {
                                Variable = new SqlVariableNode("@value"),
                                RValue = new SqlIdentifierNode("ColumnA")
                            }
                        }
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }
    }
}

