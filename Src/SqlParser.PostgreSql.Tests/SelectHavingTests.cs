//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Tokenizing;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class SelectHavingTests
//    {
//        [Test]
//        public void Select_HavingColumnEqualsNumber()
//        {
//            const string s = "SELECT * FROM mytable HAVING mycolumn = 1;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable"),
//                    HavingClause = new SqlInfixOperationNode
//                    {
//                        Left = new SqlIdentifierNode("mycolumn"),
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlNumberNode(1)
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_HavingAnd()
//        {
//            const string s = "SELECT * FROM mytable HAVING mycolumn1 = 1 AND mycolumn2 = 2;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause =  new SqlObjectIdentifierNode("mytable"),
//                    HavingClause = new SqlInfixOperationNode
//                    {
//                        Left = new SqlInfixOperationNode
//                        {
//                            Left = new SqlIdentifierNode("mycolumn1"),
//                            Operator = new SqlOperatorNode("="),
//                            Right = new SqlNumberNode(1)
//                        },
//                        Operator = new SqlOperatorNode("AND"),
//                        Right = new SqlInfixOperationNode
//                        {
//                            Left = new SqlIdentifierNode("mycolumn2"),
//                            Operator = new SqlOperatorNode("="),
//                            Right = new SqlNumberNode(2)
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_HavingColumnBetween()
//        {
//            const string s = "SELECT * FROM mytable HAVING mycolumn BETWEEN 1 AND 2;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause =  new SqlObjectIdentifierNode("mytable"),
//                    HavingClause = new SqlBetweenOperationNode
//                    {
//                        Left = new SqlIdentifierNode("mycolumn"),
//                        Low = new SqlNumberNode(1),
//                        High = new SqlNumberNode(2)
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_HavingColumnInNumberList()
//        {
//            const string s = "SELECT * FROM mytable HAVING mycolumn IN (1, 2, 3);";
//            var target = new Parser();
//            var result = target.Parse(s);
//            var output = result.ToString();
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOperatorNode("*")
//                    },
//                    FromClause =  new SqlObjectIdentifierNode("mytable"),
//                    HavingClause = new SqlInNode
//                    {
//                        Search = new SqlIdentifierNode("mycolumn"),
//                        Items = new SqlListNode<ISqlNode>
//                        {
//                            new SqlNumberNode(1),
//                            new SqlNumberNode(2),
//                            new SqlNumberNode(3)
//                        }
//                    }
//                }
//            );
//        }
//    }
//}