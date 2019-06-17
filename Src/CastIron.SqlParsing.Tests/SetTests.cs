using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SetTests
    {
        [Test]
        public void Set_Number()
        {
            const string s = "SET @var = 5;";
            var target = new SqlParser();
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
            var target = new SqlParser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Operator = new SqlOperatorNode("="),
                    Right = new SqlParenthesisNode<SqlNode>
                    {
                        Expression = new SqlSelectNode
                        {
                            Columns = new SqlListNode<SqlNode>
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
            var target = new SqlParser();
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