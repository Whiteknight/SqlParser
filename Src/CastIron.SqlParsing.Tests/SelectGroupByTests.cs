using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SelectGroupByTests
    {
        [Test]
        public void Select_GroupByColumn()
        {
            const string s = "SELECT * FROM TableA GROUP BY Column1";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

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