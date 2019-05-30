using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using FluentAssertions;
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

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<SqlNode>
                    {
                        new SqlStarNode()
                    },
                    FromClause = new SqlSelectFromClauseNode
                    {
                        Source = new SqlIdentifierNode("TableA")
                    },
                    GroupByClause = new SqlSelectGroupByNode
                    {
                        Keys = new SqlListNode<SqlNode>
                        {
                            new SqlIdentifierNode("Column1")
                        }
                    }
                }
            );
        }
    }
}