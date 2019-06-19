using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.Optimizer;
using SqlParser.Tests.Utility;

namespace SqlParser.Tests
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
        public void Optimize_AdditionMultiplicationConstants()
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
            result.Should().MatchAst(new SqlNumberNode(7));
        }

        [Test]
        public void Optimize_ConstantStringConcat()
        {
            // "A" + "B" => "AB"
            var ast = new SqlInfixOperationNode
            {
                Left = new SqlStringNode("A"),
                Operator = new SqlOperatorNode("+"),
                Right = new SqlStringNode("B")
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlStringNode("AB"));
        }

        [Test]
        public void Optimize_CastNumberToVarcharMax()
        {
            // "A" + "B" => "AB"
            var ast = new SqlCastNode
            {
                Expression = new SqlNumberNode(5),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("VARCHAR"),
                    Size = new SqlKeywordNode("MAX")
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlStringNode("5"));
        }

        [Test]
        public void Optimize_CastNumberToVarchar3()
        {
            // "A" + "B" => "AB"
            var ast = new SqlCastNode
            {
                Expression = new SqlNumberNode(12345),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("VARCHAR"),
                    Size = new SqlNumberNode(3)
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlStringNode("123"));
        }

        [Test]
        public void Optimize_CastStringToVarchar3()
        {
            // "A" + "B" => "AB"
            var ast = new SqlCastNode
            {
                Expression = new SqlStringNode("12345"),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("VARCHAR"),
                    Size = new SqlNumberNode(3)
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlStringNode("123"));
        }

        [Test]
        public void Optimize_CastStringToInt()
        {
            // "A" + "B" => "AB"
            var ast = new SqlCastNode
            {
                Expression = new SqlStringNode("12345"),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("INT")
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlNumberNode(12345));
        }

        [Test]
        public void Optimize_CastStringToBigint()
        {
            // "A" + "B" => "AB"
            var ast = new SqlCastNode
            {
                Expression = new SqlStringNode("12345"),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("BIGINT")
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlNumberNode(12345L));
        }

        [Test]
        public void Optimize_CastStringToNumeric()
        {
            // "A" + "B" => "AB"
            var ast = new SqlCastNode
            {
                Expression = new SqlStringNode("123.45"),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("NUMERIC")
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlNumberNode(123.45M));
        }

    }
}
