using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class UnionTests
    {
        [Test]
        public void Select_UnionSelect()
        {
            const string s = "SELECT * FROM Table1 UNION SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlUnionStatementNode;
            statement.Should().NotBeNull();
            statement.Operator.Should().Be("UNION");

            var select1 = statement.First as SqlSelectNode;
            select1.Columns.Count.Should().Be(1);
            select1.Columns[0].Should().BeOfType<SqlStarNode>();
            select1.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select1.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table1");

            var select2 = statement.Second as SqlSelectNode;
            select2.Columns.Count.Should().Be(1);
            select2.Columns[0].Should().BeOfType<SqlStarNode>();
            select2.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select2.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table2");
        }

        [Test]
        public void Select_UnionSelect2x()
        {
            const string s = "SELECT * FROM Table1 UNION SELECT * FROM Table2 UNION ALL SELECT * FROM Table3;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var union1 = (result as SqlStatementListNode)?.Statements?.First() as SqlUnionStatementNode;
            union1.Should().NotBeNull();
            union1.Operator.Should().Be("UNION");

            var select1 = union1.First as SqlSelectNode;
            select1.Columns.Count.Should().Be(1);
            select1.Columns[0].Should().BeOfType<SqlStarNode>();
            select1.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select1.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table1");

            var union2 = union1.Second as SqlUnionStatementNode;
            union2.Operator.Should().Be("UNION ALL");

            var select2 = union2.First as SqlSelectNode;
            select2.Columns.Count.Should().Be(1);
            select2.Columns[0].Should().BeOfType<SqlStarNode>();
            select2.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select2.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table2");

            var select3 = union2.Second as SqlSelectNode;
            select3.Columns.Count.Should().Be(1);
            select3.Columns[0].Should().BeOfType<SqlStarNode>();
            select3.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select3.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select3.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table3");
        }

        [Test]
        public void Select_UnionAllSelect()
        {
            const string s = "SELECT * FROM Table1 UNION ALL SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlUnionStatementNode;
            statement.Should().NotBeNull();
            statement.Operator.Should().Be("UNION ALL");

            var select1 = statement.First as SqlSelectNode;
            select1.Columns.Count.Should().Be(1);
            select1.Columns[0].Should().BeOfType<SqlStarNode>();
            select1.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select1.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table1");

            var select2 = statement.Second as SqlSelectNode;
            select2.Columns.Count.Should().Be(1);
            select2.Columns[0].Should().BeOfType<SqlStarNode>();
            select2.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select2.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table2");
        }

        [Test]
        public void Select_ExceptSelect()
        {
            const string s = "SELECT * FROM Table1 EXCEPT SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlUnionStatementNode;
            statement.Should().NotBeNull();
            statement.Operator.Should().Be("EXCEPT");

            var select1 = statement.First as SqlSelectNode;
            select1.Columns.Count.Should().Be(1);
            select1.Columns[0].Should().BeOfType<SqlStarNode>();
            select1.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select1.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table1");

            var select2 = statement.Second as SqlSelectNode;
            select2.Columns.Count.Should().Be(1);
            select2.Columns[0].Should().BeOfType<SqlStarNode>();
            select2.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select2.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table2");
        }

        [Test]
        public void Select_IntersectSelect()
        {
            const string s = "SELECT * FROM Table1 INTERSECT SELECT * FROM Table2";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlUnionStatementNode;
            statement.Should().NotBeNull();
            statement.Operator.Should().Be("INTERSECT");

            var select1 = statement.First as SqlSelectNode;
            select1.Columns.Count.Should().Be(1);
            select1.Columns[0].Should().BeOfType<SqlStarNode>();
            select1.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select1.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select1.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table1");

            var select2 = statement.Second as SqlSelectNode;
            select2.Columns.Count.Should().Be(1);
            select2.Columns[0].Should().BeOfType<SqlStarNode>();
            select2.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select2.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("Table2");
        }
    }
}