using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Validation;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class ValidationTests
    {
        [Test]
        public void Validate_Alias()
        {
            var ast = new SqlAliasNode
            {
                Alias = null,
                Source = null
            };
            ast.Validate().Passed.Should().BeFalse();

            ast = new SqlAliasNode
            {
                Alias = new SqlIdentifierNode("x"),
                Source = new SqlIdentifierNode("MyTable")
            };
            ast.Validate().Passed.Should().BeTrue();
        }

        [Test]
        public void Validate_Between()
        {
            var ast = new SqlBetweenOperationNode
            {
                Left = new SqlIdentifierNode("a"),
                Low = new SqlNumberNode(5),
                High = new SqlKeywordNode("MAX")
            };
            var result = ast.Validate();
            result.Passed.Should().BeFalse();

            ast = new SqlBetweenOperationNode
            {
                Left = new SqlIdentifierNode("a"),
                Low = new SqlNumberNode(5),
                High = new SqlNumberNode(7)
            };
            ast.Validate().Passed.Should().BeTrue();
        }
    }
}
