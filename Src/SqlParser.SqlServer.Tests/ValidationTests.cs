using FluentAssertions;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Validation;
using SqlParser.SqlStandard;

namespace SqlParser.SqlServer.Tests
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
            ast.ValidateForSqlServer().Passed.Should().BeFalse();

            ast = new SqlAliasNode
            {
                Alias = new SqlIdentifierNode("x"),
                Source = new SqlIdentifierNode("MyTable")
            };
            ast.ValidateForSqlServer().Passed.Should().BeTrue();
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
            var result = ast.ValidateForSqlServer();
            result.Passed.Should().BeFalse();

            ast = new SqlBetweenOperationNode
            {
                Left = new SqlIdentifierNode("a"),
                Low = new SqlNumberNode(5),
                High = new SqlNumberNode(7)
            };
            ast.ValidateForSqlServer().Passed.Should().BeTrue();
        }
    }
}
