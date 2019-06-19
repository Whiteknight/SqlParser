using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests
{
    [TestFixture]
    public class InsertSelectTests
    {
        [Test]
        public void Insert_ValuesOneRowTwoColumns()
        {
            const string s = @"INSERT INTO MyTable(Column1, Column2) SELECT ColumnA, ColumnB FROM MyTable
;";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
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
