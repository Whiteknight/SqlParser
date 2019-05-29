using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectOrderByTests
    {
        [Test]
        public void Select_OrderByColumnDesc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn DESC;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.OrderBy.Should().NotBeNull();
            statement.OrderBy.Entries.Count().Should().Be(1);
            (statement.OrderBy.Entries[0].Source as SqlIdentifierNode).Name.Should().Be("MyColumn");
            statement.OrderBy.Entries[0].Direction.Should().Be("DESC");
        }

        [Test]
        public void Select_OrderByNumberDesc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY 1 DESC;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.OrderBy.Should().NotBeNull();
            statement.OrderBy.Entries.Count().Should().Be(1);
            (statement.OrderBy.Entries[0].Source as SqlNumberNode).Value.Should().Be(1);
            statement.OrderBy.Entries[0].Direction.Should().Be("DESC");
        }

        [Test]
        public void Select_OrderByColumnAsc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn ASC;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.OrderBy.Should().NotBeNull();
            statement.OrderBy.Entries.Count().Should().Be(1);
            (statement.OrderBy.Entries[0].Source as SqlIdentifierNode).Name.Should().Be("MyColumn");
            statement.OrderBy.Entries[0].Direction.Should().Be("ASC");
        }

        [Test]
        public void Select_OrderByColumnNone()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.OrderBy.Should().NotBeNull();
            statement.OrderBy.Entries.Count().Should().Be(1);
            (statement.OrderBy.Entries[0].Source as SqlIdentifierNode).Name.Should().Be("MyColumn");
            statement.OrderBy.Entries[0].Direction.Should().BeNullOrEmpty();
        }

        [Test]
        public void Select_OrderByColumnNoneOffsetNumberFetchNumber()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn OFFSET 5 ROWS FETCH NEXT 10 ROWS ONLY;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.OrderBy.Should().NotBeNull();
            statement.OrderBy.Entries.Count().Should().Be(1);
            (statement.OrderBy.Entries[0].Source as SqlIdentifierNode).Name.Should().Be("MyColumn");
            statement.OrderBy.Entries[0].Direction.Should().BeNullOrEmpty();

            (statement.OrderBy.Offset as SqlNumberNode).Value.Should().Be(5);
            (statement.OrderBy.Limit as SqlNumberNode).Value.Should().Be(10);
        }

        [Test]
        public void Select_OrderByColumnAscDesc()
        {
            const string s = "SELECT * FROM MyTable ORDER BY MyColumn1 ASC, MyColumn2 DESC;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");

            statement.OrderBy.Should().NotBeNull();
            statement.OrderBy.Entries.Count().Should().Be(2);
            (statement.OrderBy.Entries[0].Source as SqlIdentifierNode).Name.Should().Be("MyColumn1");
            statement.OrderBy.Entries[0].Direction.Should().Be("ASC");
            (statement.OrderBy.Entries[1].Source as SqlIdentifierNode).Name.Should().Be("MyColumn2");
            statement.OrderBy.Entries[1].Direction.Should().Be("DESC");
        }
    }
}