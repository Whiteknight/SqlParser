using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tests.SqlServer.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.Tests.SqlServer
{
    [TestFixture]
    public class DeleteTests
    {
        [Test]
        public void Delete_FromTable()
        {
            const string s = "DELETE FROM MyTable;";
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

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
            var target = new Parser();
            var result = target.Parse(new Tokenizer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlDeleteNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable"),
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("ColumnA"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(1)
                    }
                }
            );
        }
    }
}
