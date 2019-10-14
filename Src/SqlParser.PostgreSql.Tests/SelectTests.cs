using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class SelectTests
    {
        [Test]
        public void Select_StringConstant()
        {
            const string s = "SELECT 'TEST'";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        Children = new List<ISqlNode>
                        {
                            new SqlStringNode("TEST")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_StringConstantAlias()
        {
            const string s = "SELECT 'TEST' AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        Children = new List<ISqlNode>
                        {
                            new SqlAliasNode
                            {
                                Source = new SqlStringNode("TEST"),
                                Alias = new SqlIdentifierNode("ColumnA")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_IntegerConstant()
        {
            const string s = "SELECT 10";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        Children = new List<ISqlNode>
                        {
                            new SqlNumberNode(10)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_NegativeIntegerConstant()
        {
            const string s = "SELECT -10";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("-"),
                            Right = new SqlNumberNode(10)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_PrefixesIntegerConstant()
        {
            const string s = "SELECT ~+-10";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("~"),
                            Right = new SqlPrefixOperationNode
                            {
                                Operator = new SqlOperatorNode("+"),
                                Right = new SqlPrefixOperationNode
                                {
                                    Operator = new SqlOperatorNode("-"),
                                    Right = new SqlNumberNode(10)
                                }
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_DecimalConstant()
        {
            const string s = "SELECT 10.123";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        Children = new List<ISqlNode>
                        {
                            new SqlNumberNode(10.123M)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_Variable()
        {
            const string s = "SELECT @value";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlVariableNode("@value")
                    }
                }
            );
        }

        [Test]
        public void Select_VariableAllChars()
        {
            const string s = "SELECT @$#_abc123@";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlVariableNode("@$#_abc123@")
                    }
                }
            );
        }

        [Test]
        public void Select_NegativeVariable()
        {
            const string s = "SELECT -@value";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode("-"),
                            Right = new SqlVariableNode("@value")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_VariableAlias()
        {
            const string s = "SELECT @value AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlVariableNode("@value"),
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_FunctionCall()
        {
            const string s = "SELECT GETUTCDATE() AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("GETUTCDATE")
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_CountStar()
        {
            const string s = "SELECT COUNT(*) AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlKeywordNode("COUNT"),
                                Arguments = new SqlListNode<ISqlNode>
                                {
                                    new SqlOperatorNode("*")
                                }
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_FunctionCallArgument()
        {
            const string s = "SELECT ABS(1) AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("ABS"),
                                Arguments = new SqlListNode<ISqlNode>
                                {
                                    new SqlNumberNode(1)
                                }
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_FunctionCall2Arguments()
        {
            const string s = "SELECT COALESCE(NULL, 0) AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("COALESCE"),
                                Arguments = new SqlListNode<ISqlNode>
                                {
                                    new SqlNullNode(),
                                    new SqlNumberNode(0)
                                }
                            },
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_Null()
        {
            const string s = "SELECT NULL AS ColumnA";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Source = new SqlNullNode(),
                            Alias = new SqlIdentifierNode("ColumnA")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_TwoStatementsUnseparated()
        {
            const string s = "SELECT 'TEST1' SELECT 'TEST2'";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Should().MatchAst(
                new SqlStatementListNode
                {
                    new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST1")
                            }
                        }
                    },
                    new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST2")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_TwoStatementsSeparated()
        {
            const string s = "SELECT 'TEST1'; SELECT 'TEST2'";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Should().MatchAst(
                new SqlStatementListNode
                {
                    new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST1")
                            }
                        }
                    },
                    new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlStringNode("TEST2")
                            }
                        }
                    }
                }
            );
        }
    }
}