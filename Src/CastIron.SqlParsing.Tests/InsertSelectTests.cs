using System.Collections.Generic;
using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class InsertSelectTests
    {
        [Test]
        public void Insert_ValuesOneRowTwoColumns()
        {
            const string s = @"INSERT INTO MyTable(Column1, Column2) SELECT ColumnA, ColumnB FROM MyTable
;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

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
                        Columns = new SqlListNode<SqlNode>
                        {
                            Children = new List<SqlNode>
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
    }
}
