using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public void Update_SetWhere()
        {
            const string s = "UPDATE MyTable SET ColumnA = 1 WHERE ColumnB = 2;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlUpdateNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable"),
                    SetClause = new SqlListNode<SqlInfixOperationNode>
                    {
                        new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("ColumnA"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        }
                    },
                    WhereClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("ColumnB"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(2)
                    }
                }
            );
        }

        [Test]
        public void Update_SetNull()
        {
            const string s = "UPDATE MyTable SET ColumnA = NULL;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlUpdateNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable"),
                    SetClause = new SqlListNode<SqlInfixOperationNode>
                    {
                        new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("ColumnA"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNullNode()
                        }
                    }
                }
            );
        }

        [Test]
        public void Update_SetDefault()
        {
            const string s = "UPDATE MyTable SET ColumnA = DEFAULT;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlUpdateNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable"),
                    SetClause = new SqlListNode<SqlInfixOperationNode>
                    {
                        new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("ColumnA"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlKeywordNode("DEFAULT")
                        }
                    }
                }
            );
        }
    }
}