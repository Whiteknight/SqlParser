using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tests.SqlServer.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests.SqlServer
{
    [TestFixture]
    public class SelectCaseTests
    { 

        [Test]
        public void Select_CaseWhenThenElseEnd()
        {
            const string s = "SELECT CASE 5 WHEN 6 THEN 'A' ELSE 'B' END;";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();
            var output = result.ToString();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlCaseNode
                        {
                            InputExpression = new SqlNumberNode(5),
                            WhenExpressions = new List<SqlCaseWhenNode>
                            {
                                new SqlCaseWhenNode
                                {
                                    Condition = new SqlNumberNode(6),
                                    Result = new SqlStringNode("A")
                                }
                            },
                            ElseExpression = new SqlStringNode("B")
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_CaseWhenThenElseEndAlias()
        {
            const string s = "SELECT CASE ValueA WHEN 6 THEN 'A' ELSE 'B' END AS ColumnA, ColumnB;";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlAliasNode
                        {
                            Alias = new SqlIdentifierNode("ColumnA"),
                            Source = new SqlCaseNode
                            {
                                InputExpression = new SqlIdentifierNode("ValueA"),
                                WhenExpressions = new List<SqlCaseWhenNode>
                                {
                                    new SqlCaseWhenNode
                                    {
                                        Condition = new SqlNumberNode(6),
                                        Result = new SqlStringNode("A")
                                    }
                                },
                                ElseExpression = new SqlStringNode("B")
                            }
                        },
                        new SqlIdentifierNode("ColumnB")
                    }
                }
            );
        }
    }
}