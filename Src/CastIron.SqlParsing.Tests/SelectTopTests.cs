using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectTopTests
    {
        [Test]
        public void Select_TopNumber()
        {
            const string s = "SELECT TOP 10 * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlNumberNode>();
            top.Percent.Should().BeFalse();
            top.WithTies.Should().BeFalse();
            (top.Value as SqlNumberNode).Value.Should().Be(10);
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_TopVariable()
        {
            const string s = "SELECT TOP @limit * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlVariableNode>();
            (top.Value as SqlVariableNode).Name.Should().Be("@limit");
            top.Percent.Should().BeFalse();
            top.WithTies.Should().BeFalse();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_TopNumberParens()
        {
            const string s = "SELECT TOP (10) * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlNumberNode>();
            (top.Value as SqlNumberNode).Value.Should().Be(10);
            top.Percent.Should().BeFalse();
            top.WithTies.Should().BeFalse();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_TopVariableParens()
        {
            const string s = "SELECT TOP (@limit) * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlVariableNode>();
            (top.Value as SqlVariableNode).Name.Should().Be("@limit");
            top.Percent.Should().BeFalse();
            top.WithTies.Should().BeFalse();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_TopNumberParensPercent()
        {
            const string s = "SELECT TOP (10) PERCENT * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlNumberNode>();
            top.Percent.Should().BeTrue();
            top.WithTies.Should().BeFalse();
            (top.Value as SqlNumberNode).Value.Should().Be(10);
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_TopNumberParensWithTies()
        {
            const string s = "SELECT TOP (10) WITH TIES * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlNumberNode>();
            top.Percent.Should().BeFalse();
            top.WithTies.Should().BeTrue();
            (top.Value as SqlNumberNode).Value.Should().Be(10);
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_TopNumberParensPercentWithTies()
        {
            const string s = "SELECT TOP (10) PERCENT WITH TIES * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            var top = statement.Top as SqlSelectTopNode;
            top.Value.Should().BeOfType<SqlNumberNode>();
            top.Percent.Should().BeTrue();
            top.WithTies.Should().BeTrue();
            (top.Value as SqlNumberNode).Value.Should().Be(10);
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }
    }
}