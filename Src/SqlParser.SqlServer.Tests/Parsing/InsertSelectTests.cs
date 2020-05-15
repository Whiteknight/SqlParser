using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.SqlStandard;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class InsertSelectTests
    {
        [Test]
        public void Insert_SelectOneRowTwoColumns()
        {
            const string s = @"INSERT INTO MyTable(Column1, Column2) SELECT ColumnA, ColumnB FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1"),
                        new SqlIdentifierNode("Column2")
                    },
                    Source = new SqlSelectNode
                    {
                        Columns = new SqlListNode<ISqlNode>
                        {
                            Children = new List<ISqlNode>
                            {
                                new SqlIdentifierNode("ColumnA"),
                                new SqlIdentifierNode("ColumnB")
                            }
                        },
                        FromClause =  new SqlObjectIdentifierNode("MyTable")
                    }
                }
            );
        }

        [Test]
        public void Insert_Execute()
        {
            const string s = @"INSERT INTO MyTable (Column1) EXECUTE 'SELECT 1';";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1")
                    },
                    Source = new SqlExecuteNode
                    {
                        Name = new SqlStringNode("SELECT 1")
                    }
                }
            );
        }
    }
}
