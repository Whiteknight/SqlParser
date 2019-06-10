using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Optimizer;
using CastIron.SqlParsing.Tests.Utility;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class OptimizeExpressionTests
    {
        [Test]
        public void Optimize_NegativeNumber()
        {
            var ast = new SqlPrefixOperationNode
            {
                Operator = new SqlOperatorNode("-"),
                Right = new SqlNumberNode(5)
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlNumberNode(-5));
        }

        [Test]
        public void Optimize_PositiveNumber()
        {
            var ast = new SqlPrefixOperationNode
            {
                Operator = new SqlOperatorNode("+"),
                Right = new SqlNumberNode(5)
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlNumberNode(5));
        }

        [Test]
        public void Optimize_ConstantExpression()
        {
            // 1 + 2 * 3 => 7
            var ast = new SqlInfixOperationNode
            {
                Left = new SqlNumberNode(1),
                Operator = new SqlOperatorNode("+"),
                Right = new SqlInfixOperationNode
                {
                    Left = new SqlNumberNode(2),
                    Operator = new SqlOperatorNode("*"),
                    Right = new SqlNumberNode(3)
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlNumberNode());
        }
    }
}
