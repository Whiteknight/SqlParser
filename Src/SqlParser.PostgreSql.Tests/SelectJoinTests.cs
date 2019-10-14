using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class SelectJoinTests
    {
        [TestCase("JOIN")]
        [TestCase("FULL JOIN")]
        [TestCase("FULL OUTER JOIN")]
        [TestCase("CROSS APPLY")]
        [TestCase("OUTER APPLY")]
        [TestCase("CROSS JOIN")]
        [TestCase("INNER JOIN")]
        [TestCase("LEFT OUTER JOIN")]
        [TestCase("RIGHT OUTER JOIN")]
        public void Select_Join(string joinType)
        {
            string s = $@"
SELECT 
    * 
    FROM 
        ""Table1"" t1
        {joinType}
        ""Table2"" t2
            ON t1.""Id"" = t2.""Id"";";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlJoinNode
                    {
                        Left = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t1"),
                            Source = new SqlObjectIdentifierNode("Table1")
                        },
                        Operator = new SqlOperatorNode(joinType),
                        Right = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t2"),
                            Source = new SqlObjectIdentifierNode("Table2")
                        },
                        OnCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t1"),
                                Identifier = new SqlIdentifierNode("Id")
                            },
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlQualifiedIdentifierNode
                            {
                                Qualifier = new SqlIdentifierNode("t2"),
                                Identifier = new SqlIdentifierNode("Id")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_NaturalJoin()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        ""Table1"" t1
        NATURAL JOIN
        ""Table2"" t2;";
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
                    FromClause = new SqlJoinNode
                    {
                        Left = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t1"),
                            Source = new SqlObjectIdentifierNode("Table1")
                        },
                        Operator = new SqlOperatorNode("NATURAL JOIN"),
                        Right = new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("t2"),
                            Source = new SqlObjectIdentifierNode("Table2")
                        }
                    },
                }
            );
        }
    }
}