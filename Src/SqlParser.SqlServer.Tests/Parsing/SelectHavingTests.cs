using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Parsing;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class SelectHavingTests
    {
        [Test]
        public void Select_HavingColumnEqualsNumber()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn = 1;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause = new SqlObjectIdentifierNode("MyTable"),
                    HavingClause = new SqlInfixOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Operator = new SqlOperatorNode("="),
                        Right = new SqlNumberNode(1)
                    }
                }
            );
        }

        [Test]
        public void Select_HavingAnd()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn1 = 1 AND MyColumn2 = 2;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause =  new SqlObjectIdentifierNode("MyTable"),
                    HavingClause = new SqlInfixOperationNode
                    {
                        Left = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn1"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        },
                        Operator = new SqlOperatorNode("AND"),
                        Right = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("MyColumn2"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(2)
                        }
                    }
                }
            );
        }

        [Test]
        public void Select_HavingColumnBetween()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn BETWEEN 1 AND 2;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause =  new SqlObjectIdentifierNode("MyTable"),
                    HavingClause = new SqlBetweenOperationNode
                    {
                        Left = new SqlIdentifierNode("MyColumn"),
                        Low = new SqlNumberNode(1),
                        High = new SqlNumberNode(2)
                    }
                }
            );
        }

        [Test]
        public void Select_HavingColumnInNumberList()
        {
            const string s = "SELECT * FROM MyTable HAVING MyColumn IN (1, 2, 3);";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForSqlServer(s));
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlSelectNode
                {
                    Columns = new SqlListNode<ISqlNode>
                    {
                        new SqlOperatorNode("*")
                    },
                    FromClause =  new SqlObjectIdentifierNode("MyTable"),
                    HavingClause = new SqlInNode
                    {
                        Search = new SqlIdentifierNode("MyColumn"),
                        Items = new SqlListNode<ISqlNode>
                        {
                            new SqlNumberNode(1),
                            new SqlNumberNode(2),
                            new SqlNumberNode(3)
                        }
                    }
                }
            );
        }
    }
}