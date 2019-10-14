using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.Optimizer;
using SqlParser.PostgreSql.Tests.Utility;

namespace SqlParser.PostgreSql.Tests
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
            // CAST(5 as VARCHAR(MAX)) => "5"
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
            // CAST(12345 as VARCHAR(3)) => "123"
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
            // CAST("12345" as VARCHAR(3)) => "123"
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
        public void Optimize_CastStringToCharPadRight()
        {
            // CAST("12345" as VARCHAR(3)) => "123"
            var ast = new SqlCastNode
            {
                Expression = new SqlStringNode("ab"),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("CHAR"),
                    Size = new SqlNumberNode(4)
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlStringNode("ab  "));
        }

        [Test]
        public void Optimize_CastStringToVarcharNoPadRight()
        {
            // CAST("12345" as VARCHAR(3)) => "123"
            var ast = new SqlCastNode
            {
                Expression = new SqlStringNode("ab"),
                DataType = new SqlDataTypeNode
                {
                    DataType = new SqlKeywordNode("VARCHAR"),
                    Size = new SqlNumberNode(4)
                }
            };

            var target = new ExpressionOptimizeVisitor();
            var result = target.Visit(ast);
            result.Should().MatchAst(new SqlStringNode("ab"));
        }

        [Test]
        public void Optimize_CastStringToInt()
        {
            // CAST("12345" as INT) => 12345
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
            // CAST("12345" as BIGINT) => 12345L
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
            // CAST("123.45" as NUMERIC) => 123.45M
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
