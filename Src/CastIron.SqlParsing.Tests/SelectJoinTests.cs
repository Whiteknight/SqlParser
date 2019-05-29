using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectJoinTests
    {
        [Test]
        public void Select_Join()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1 t1
        JOIN
        Table2 t2
            ON t1.Id = t2.Id;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("JOIN");
            var condition = join.OnCondition as SqlInfixOperationNode;
            ((condition.Left as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            ((condition.Right as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_FullJoin()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1 t1
        FULL JOIN
        Table2 t2
            ON t1.Id = t2.Id;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("FULL JOIN");
            var condition = join.OnCondition as SqlInfixOperationNode;
            ((condition.Left as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            ((condition.Right as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_FullOuterJoin()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1 t1
        FULL OUTER JOIN
        Table2 t2
            ON t1.Id = t2.Id;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("FULL OUTER JOIN");
            var condition = join.OnCondition as SqlInfixOperationNode;
            ((condition.Left as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            ((condition.Right as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_FullJoinNoAlias()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1
        FULL JOIN
        Table2 
            ON Column1 = Column2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlIdentifierNode).Name.Should().Be("Table1");
            (join.Right as SqlIdentifierNode).Name.Should().Be("Table2");
            join.Operator.Operator.Should().Be("FULL JOIN");
            var condition = join.OnCondition as SqlInfixOperationNode;
            (condition.Left as SqlIdentifierNode).Name.Should().Be("Column1");
            (condition.Right as SqlIdentifierNode).Name.Should().Be("Column2");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_LeftInnerJoin()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1 t1
        LEFT INNER JOIN
        Table2 t2
            ON t1.Id = t2.Id;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("LEFT INNER JOIN");
            var condition = join.OnCondition as SqlInfixOperationNode;
            ((condition.Left as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            ((condition.Right as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_RightOuterJoin()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1 t1
        RIGHT OUTER JOIN
        Table2 t2
            ON t1.Id = t2.Id;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("RIGHT OUTER JOIN");
            var condition = join.OnCondition as SqlInfixOperationNode;
            ((condition.Left as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            ((condition.Right as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_CrossApply()
        {
            const string s = @"

SELECT 
    * 
    FROM 
        Table1 t1
        CROSS APPLY
        Table2 t2
            ON t1.Id = t2.Id;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("CROSS APPLY");
            var condition = join.OnCondition as SqlInfixOperationNode;
            ((condition.Left as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            ((condition.Right as SqlQualifiedIdentifierNode).Identifier as SqlIdentifierNode).Name.Should().Be("Id");
            (condition.Operator as SqlOperatorNode).Operator.Should().Be("=");
        }

        [Test]
        public void Select_NaturalJoin()
        {
            const string s = @"
SELECT 
    * 
    FROM 
        Table1 t1
        NATURAL JOIN
        Table2 t2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();

            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var join = statement.FromClause.Source as SqlJoinNode;
            (join.Left as SqlAliasNode).Alias.Name.Should().Be("t1");
            (join.Right as SqlAliasNode).Alias.Name.Should().Be("t2");
            join.Operator.Operator.Should().Be("NATURAL JOIN");

            join.OnCondition.Should().BeNull();
        }
    }
}