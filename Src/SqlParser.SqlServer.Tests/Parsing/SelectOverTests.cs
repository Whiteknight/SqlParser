using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class SelectOverTests
    {
        [Test]
        public void Select_RowNumberOverPartition()
        {
            const string s = "SELECT ROW_NUMBER() OVER (PARTITION BY ColumnA)";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOverNode
                        {
                            Expression = new SqlFunctionCallNode
                            {
                                Name = new SqlIdentifierNode("ROW_NUMBER")
                            },
                            PartitionBy = new SqlListNode<ISqlNode>
                            {
                                new SqlIdentifierNode("ColumnA")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_AvgOverOrderBy()
        {
            const string s = "SELECT AVG(ColumnA) OVER (ORDER BY ColumnA) FROM MyTable";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOverNode
                        {
                            Expression = new SqlFunctionCallNode
                            {
                                Name = new SqlKeywordNode("AVG"),
                                Arguments = new SqlListNode<ISqlNode>
                                {
                                    new SqlIdentifierNode("ColumnA")
                                }
                            },
                            OrderBy = new SqlListNode<SqlOrderByEntryNode>
                            {
                                new SqlOrderByEntryNode
                                {
                                    Source = new SqlIdentifierNode("ColumnA")
                                }
                            }
                        }
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable")
                }
            );
        }
    }
}