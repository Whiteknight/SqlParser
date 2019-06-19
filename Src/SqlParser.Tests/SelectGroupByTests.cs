using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests
{
    [TestFixture]
    public class SelectGroupByTests
    {
        [Test]
        public void Select_GroupByColumn()
        {
            const string s = "SELECT * FROM TableA GROUP BY Column1";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("TableA"),
                    GroupByClause = new SqlListNode<SqlNode>
                    {
                        new SqlIdentifierNode("Column1")
                    }
                }
            );
        }
    }
}