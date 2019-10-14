using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tests.Utility;

namespace SqlParser.Tests.SqlServer
{
    [TestFixture]
    public class SetTests
    {
        [Test]
        public void Set_Number()
        {
            const string s = "SET @var = 5;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Operator = new SqlOperatorNode("="),
                    Right = new SqlNumberNode(5)
                }
            );
        }

        [Test]
        public void Set_SelectExpression()
        {
            const string s = "SET @var = (SELECT 5);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Operator = new SqlOperatorNode("="),
                    Right = new SqlParenthesisNode<ISqlNode>
                    {
                        Expression = new SqlSelectNode
                        {
                            Columns = new SqlListNode<ISqlNode>
                            {
                                new SqlNumberNode(5)
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Set_CompoundOperator()
        {
            const string s = "SET @var += 5;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Operator = new SqlOperatorNode("+="),
                    Right = new SqlNumberNode(5)
                }
            );
        }
    }
}