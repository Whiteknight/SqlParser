using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class DeleteTests
    {
        [Test]
        public void Delete_FromTable()
        {
            const string s = "DELETE FROM MyTable;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlDeleteNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable")
                }
            );
        }

        [Test]
        public void Delete_FromTableWhereCondition()
        {
            const string s = "DELETE FROM MyTable WHERE ColumnA = 1;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlDeleteNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlWhereNode
                    {
                        SearchCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("ColumnA"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        }
                    }
                }
            );
        }
    }
}
