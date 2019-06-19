using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SqlParser.Tokenizing;

namespace SqlParser.Tests
{
    [TestFixture]
    public class SqlTokenizerTests
    {
        [Test]
        public void Tokenize_WithLocation()
        {
            const string s = "SELECT * FROM MyTable";
            var target = new SqlTokenizer(s);
            var result = target.ToList();

            result[0].Type.Should().Be(SqlTokenType.Keyword);
            result[0].Value.Should().Be("SELECT");
            result[0].Location.Line.Should().Be(1);
            result[0].Location.Column.Should().Be(1);

            result[1].Type.Should().Be(SqlTokenType.Whitespace);

            result[2].Type.Should().Be(SqlTokenType.Symbol);
            result[2].Value.Should().Be("*");
            result[2].Location.Line.Should().Be(1);
            result[2].Location.Column.Should().Be(8);

            result[3].Type.Should().Be(SqlTokenType.Whitespace);

            result[4].Type.Should().Be(SqlTokenType.Keyword);
            result[4].Value.Should().Be("FROM");
            result[4].Location.Line.Should().Be(1);
            result[4].Location.Column.Should().Be(10);

            result[5].Type.Should().Be(SqlTokenType.Whitespace);

            result[6].Type.Should().Be(SqlTokenType.Identifier);
            result[6].Value.Should().Be("MyTable");
            result[6].Location.Line.Should().Be(1);
            result[6].Location.Column.Should().Be(15);
        }
    }
}
