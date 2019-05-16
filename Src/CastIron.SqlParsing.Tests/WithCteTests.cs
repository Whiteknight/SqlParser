using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class WithCteTests
    {
        [Test]
        public void With_Select()
        {
            const string s = @"
WITH 
Cte1 AS (
    SELECT * FROM MyTable
)
SELECT * FROM Cte1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode).Statements[0] as SqlWithNode;
            statement.Ctes.Count.Should().Be(1);
            statement.Ctes[0].Name.Name.Should().Be("Cte1");
            var select1 = statement.Ctes[0].Select as SqlSelectNode;
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
            var select2 = statement.Statement as SqlSelectNode;
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Cte1");
        }

        [Test]
        public void With_SelectTwoCtes()
        {
            const string s = @"
WITH 
Cte1 AS (
    SELECT * FROM MyTable
),
Cte2 AS (
    SELECT * FROM Cte1
)
SELECT * FROM Cte2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode).Statements[0] as SqlWithNode;
            statement.Ctes.Count.Should().Be(2);

            statement.Ctes[0].Name.Name.Should().Be("Cte1");
            var select1 = statement.Ctes[0].Select as SqlSelectNode;
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.Ctes[1].Name.Name.Should().Be("Cte2");
            var select2 = statement.Ctes[1].Select as SqlSelectNode;
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Cte1");

            var select3 = statement.Statement as SqlSelectNode;
            (select3.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Cte2");
        }
    }
}