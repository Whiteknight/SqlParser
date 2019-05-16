using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SimpleSelectTests
    {
        [Test]
        public void Select_StarFromTable()
        {
            const string s = "SELECT * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_StarFromSchemaTable()
        {
            const string s = "SELECT * FROM dbo.MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            var table = statement.FromClause.Source as SqlTableIdentifierNode;
            table.Name.Should().Be("MyTable");
            table.Schema.Name.Should().Be("dbo");
        }

        [Test]
        public void Select_SingleLineComments()
        {
            const string s = @"
            -- Before
            SELECT 
                -- FirstColumn,
                * 
                FROM 
                    MyTable
                    -- INNER JOIN SomeOtherTable ON MyTableId = SomeOtherTableId;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_MultiLineComments()
        {
            const string s = @"
            /*
             BEFORE
            */
            SELECT 
                /* FirstColumn, */  * 
                FROM 
                    MyTable /* INNER JOIN SomeOtherTable ON MyTableId = SomeOtherTableId */;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_DistinctStarFromTable()
        {
            const string s = "SELECT DISTINCT * FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Modifier.Should().Be("DISTINCT");
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_StarFromTableVariable()
        {
            const string s = "SELECT * FROM @tableVar;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlVariableNode>();
            (statement.FromClause.Source as SqlVariableNode).Name.Should().Be("@tableVar");
        }

        [Test]
        public void Select_TableAlias()
        {
            const string s = "SELECT t1.* FROM MyTable AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlColumnIdentifierNode>();
            var column1 = statement.Columns[0] as SqlColumnIdentifierNode;
            column1.Table.Name.Should().Be("t1");
            column1.Column.Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlAliasNode>();
            var table = (statement.FromClause.Source as SqlAliasNode).Source as SqlIdentifierNode;
            table.Name.Should().Be("MyTable");
            (statement.FromClause.Source as SqlAliasNode).Alias.Name.Should().Be("t1");
        }

        [Test]
        public void Select_TableAliasColumn()
        {
            const string s = "SELECT t1.MyColumn FROM MyTable AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlColumnIdentifierNode>();
            var column1 = statement.Columns[0] as SqlColumnIdentifierNode;
            column1.Table.Name.Should().Be("t1");
            (column1.Column as SqlIdentifierNode).Name.Should().Be("MyColumn");
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlAliasNode>();
            var table = (statement.FromClause.Source as SqlAliasNode).Source as SqlIdentifierNode;
            table.Name.Should().Be("MyTable");
            (statement.FromClause.Source as SqlAliasNode).Alias.Name.Should().Be("t1");
        }

        [Test]
        public void Select_TableAliasBracketed()
        {
            const string s = "SELECT [t1].* FROM [MyTable] AS [t1];";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlColumnIdentifierNode>();
            var column1 = statement.Columns[0] as SqlColumnIdentifierNode;
            column1.Table.Name.Should().Be("t1");
            column1.Column.Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlAliasNode>();
            var table = (statement.FromClause.Source as SqlAliasNode).Source as SqlIdentifierNode;
            table.Name.Should().Be("MyTable");
            (statement.FromClause.Source as SqlAliasNode).Alias.Name.Should().Be("t1");
        }

        [Test]
        public void Select_TableVariableAlias()
        {
            const string s = "SELECT t1.* FROM @myTable AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            statement.Columns[0].Should().BeOfType<SqlColumnIdentifierNode>();
            var column1 = statement.Columns[0] as SqlColumnIdentifierNode;
            column1.Table.Name.Should().Be("t1");
            column1.Column.Should().BeOfType<SqlStarNode>();
            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlAliasNode>();
            var table = (statement.FromClause.Source as SqlAliasNode).Source as SqlVariableNode;
            table.Name.Should().Be("@myTable");
            (statement.FromClause.Source as SqlAliasNode).Alias.Name.Should().Be("t1");
        }

        [Test]
        public void Select_TableExpression()
        {
            const string s = @"
                SELECT 
                    t1.* 
                    FROM 
                        (SELECT * FROM MyTable) AS t1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var select1 = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            select1.Should().NotBeNull();
            select1.Columns.Count.Should().Be(1);
            select1.Columns[0].Should().BeOfType<SqlColumnIdentifierNode>();
            var column1 = select1.Columns[0] as SqlColumnIdentifierNode;
            column1.Table.Name.Should().Be("t1");
            column1.Column.Should().BeOfType<SqlStarNode>();
            select1.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();

            select1.FromClause.Source.Should().BeOfType<SqlAliasNode>();
            (select1.FromClause.Source as SqlAliasNode).Alias.Name.Should().Be("t1");

            var subexpression = (select1.FromClause.Source as SqlAliasNode).Source as SqlSelectSubexpressionNode;
            var select2 = subexpression.Select as SqlSelectNode;
            select2.Columns.Count.Should().Be(1);
            select2.Columns[0].Should().BeOfType<SqlStarNode>();
            select2.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            select2.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (select2.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_ColumnsFromTable()
        {
            const string s = "SELECT ColumnA, ColumnB FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(2);
            (statement.Columns[0] as SqlIdentifierNode).Name.Should().Be("ColumnA");
            (statement.Columns[1] as SqlIdentifierNode).Name.Should().Be("ColumnB");


            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_IdentityRowGuidFromTable()
        {
            const string s = "SELECT $IDENTITY, $ROWGUID FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(2);
            (statement.Columns[0] as SqlIdentifierNode).Name.Should().Be("$IDENTITY");
            (statement.Columns[1] as SqlIdentifierNode).Name.Should().Be("$ROWGUID");


            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_BracketedColumnsFromTable()
        {
            const string s = "SELECT [ColumnA], [ColumnB] FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(2);
            (statement.Columns[0] as SqlIdentifierNode).Name.Should().Be("ColumnA");
            (statement.Columns[1] as SqlIdentifierNode).Name.Should().Be("ColumnB");


            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_BracketedKeywordColumnsFromTable()
        {
            const string s = "SELECT [SELECT], [FROM] FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(2);
            (statement.Columns[0] as SqlIdentifierNode).Name.Should().Be("SELECT");
            (statement.Columns[1] as SqlIdentifierNode).Name.Should().Be("FROM");


            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_ColumnAliasFromTable()
        {
            const string s = "SELECT ColumnA AS ColumnB FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            var column1 = (statement.Columns[0] as SqlAliasNode);
            (column1.Source as SqlIdentifierNode).Name.Should().Be("ColumnA");
            column1.Alias.Name.Should().Be("ColumnB");


            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_ColumnAliasBracketedFromTable()
        {
            const string s = "SELECT [ColumnA] AS [ColumnB] FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            var column1 = (statement.Columns[0] as SqlAliasNode);
            (column1.Source as SqlIdentifierNode).Name.Should().Be("ColumnA");
            column1.Alias.Name.Should().Be("ColumnB");


            statement.FromClause.Should().BeOfType<SqlSelectFromClauseNode>();
            statement.FromClause.Source.Should().BeOfType<SqlTableIdentifierNode>();
            (statement.FromClause.Source as SqlIdentifierNode).Name.Should().Be("MyTable");
        }

        [Test]
        public void Select_StringConstant()
        {
            const string s = "SELECT 'TEST'";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            (statement.Columns[0] as SqlStringNode).Value.Should().Be("TEST");
        }

        [Test]
        public void Select_StringConstantAlias()
        {
            const string s = "SELECT 'TEST' AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            var column1 = (statement.Columns[0] as SqlAliasNode);
            (column1.Source as SqlStringNode).Value.Should().Be("TEST");
            column1.Alias.Name.Should().Be("ColumnA");
        }

        [Test]
        public void Select_IntegerConstant()
        {
            const string s = "SELECT 10";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            (statement.Columns[0] as SqlNumberNode).Value.Should().Be(10);
        }

        [Test]
        public void Select_DecimalConstant()
        {
            const string s = "SELECT 10.123";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            (statement.Columns[0] as SqlNumberNode).Value.Should().Be(10.123M);
        }

        [Test]
        public void Select_Variable()
        {
            const string s = "SELECT @value";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            (statement.Columns[0] as SqlVariableNode).Name.Should().Be("@value");
        }

        [Test]
        public void Select_VariableAssign()
        {
            const string s = "SELECT @value = ColumnA FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            var assignment = (statement.Columns[0] as SqlAssignVariableNode);
            assignment.Variable.Name.Should().Be("@value");
            (assignment.RValue as SqlIdentifierNode).Name.Should().Be("ColumnA");
        }

        [Test]
        public void Select_VariableAlias()
        {
            const string s = "SELECT @value AS ColumnA";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().NotBeNull();
            var output = result.ToString();

            var statement = (result as SqlStatementListNode)?.Statements?.First() as SqlSelectNode;
            statement.Should().NotBeNull();
            statement.Columns.Count.Should().Be(1);
            var column1 = (statement.Columns[0] as SqlAliasNode);
            (column1.Source as SqlVariableNode).Name.Should().Be("@value");
            column1.Alias.Name.Should().Be("ColumnA");
        }
    }
}

