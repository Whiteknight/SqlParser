using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.Tests.Utility;

namespace SqlParser.Tests.SqlServer
{
    [TestFixture]
    public class DeclareTests
    {
        [Test]
        public void Declare_Bigint()
        {
            const string s = "DECLARE @var BIGINT;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlDeclareNode
                {
                    Variable = new SqlVariableNode("@var"),
                    DataType = new SqlDataTypeNode
                    {
                        DataType = new SqlKeywordNode("BIGINT")
                    }
                }
            );
        }

        [Test]
        public void Declare_CharSize()
        {
            const string s = "DECLARE @var CHAR(5);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlDeclareNode
                {
                    Variable = new SqlVariableNode("@var"),
                    DataType = new SqlDataTypeNode
                    {
                        DataType = new SqlKeywordNode("CHAR"),
                        Size = new SqlListNode<SqlNumberNode>
                        {
                            new SqlNumberNode(5)
                        }
                    }
                }
            );
        }

        [Test]
        public void Declare_VarcharMax()
        {
            const string s = "DECLARE @var VARCHAR(MAX);";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlDeclareNode
                {
                    Variable = new SqlVariableNode("@var"),
                    DataType = new SqlDataTypeNode
                    {
                        DataType = new SqlKeywordNode("VARCHAR"),
                        Size = new SqlKeywordNode("MAX")
                    }
                }
            );
        }
    }
}
