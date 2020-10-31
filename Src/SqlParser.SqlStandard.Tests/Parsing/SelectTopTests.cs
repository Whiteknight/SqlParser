using NUnit.Framework;
using SqlParser.SqlStandard.Tests.Utility;

namespace SqlParser.SqlStandard.Tests.Parsing
{
    [TestFixture]
    public class SelectTopTests
    {
        [Test]
        public void Select_TopNumber()
        {
            const string s = "SELECT TOP 10 * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }

        [Test]
        public void Select_TopVariable()
        {
            const string s = "SELECT TOP @limit * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }

        [Test]
        public void Select_TopNumberParens()
        {
            const string s = "SELECT TOP (10) * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }

        [Test]
        public void Select_TopVariableParens()
        {
            const string s = "SELECT TOP (@limit) * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }

        [Test]
        public void Select_TopNumberParensPercent()
        {
            const string s = "SELECT TOP (10) PERCENT * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }

        [Test]
        public void Select_TopNumberParensWithTies()
        {
            const string s = "SELECT TOP (10) WITH TIES * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }

        [Test]
        public void Select_TopNumberParensPercentWithTies()
        {
            const string s = "SELECT TOP (10) PERCENT WITH TIES * FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            // SQL Standard does not support the top clause
            result.Should().BeNull();
        }
    }
}