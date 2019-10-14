using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class SelectLimitTests
    {
        [Test]
        public void Select_LimitNumber()
        {
            const string s = "SELECT * FROM MyTable LIMIT 10;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    TopClause = new SqlTopLimitNode
                    {
                        Value = new SqlNumberNode(10)
                    }
                }
            );
        }

        [Test]
        public void Select_LimitVariable()
        {
            const string s = "SELECT * FROM MyTable LIMIT @limit;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    TopClause = new SqlTopLimitNode
                    {
                        Value = new SqlVariableNode("@limit")
                    }
                }
            );
        }

        [Test]
        public void Select_LimitNumberParens()
        {
            const string s = "SELECT * FROM MyTable LIMIT(10);";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    TopClause = new SqlTopLimitNode
                    {
                        Value = new SqlNumberNode(10)
                    }
                }
            );
        }

        [Test]
        public void Select_LimitVariableParens()
        {
            const string s = "SELECT * FROM MyTable LIMIT (@limit);";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    TopClause = new SqlTopLimitNode
                    {
                        Value = new SqlVariableNode("@limit")
                    }
                }
            );
        }
    }
}