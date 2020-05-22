//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Tokenizing;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class UpdateTests
//    {
//        [Test]
//        public void Update_SetWhere()
//        {
//            const string s = "UPDATE mytable SET columna = 1 WHERE columnb = 2;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            var output = result.ToString();
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlUpdateNode
//                {
//                    Source = new SqlObjectIdentifierNode("mytable"),
//                    SetClause = new SqlListNode<SqlInfixOperationNode>
//                    {
//                        new SqlInfixOperationNode
//                        {
//                            Left = new SqlIdentifierNode("columna"),
//                            Operator = new SqlOperatorNode("="),
//                            Right = new SqlNumberNode(1)
//                        }
//                    },
//                    WhereClause = new SqlInfixOperationNode
//                    {
//                        Left = new SqlIdentifierNode("columnb"),
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlNumberNode(2)
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Update_SetNull()
//        {
//            const string s = "UPDATE mytable SET columna = NULL;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlUpdateNode
//                {
//                    Source = new SqlObjectIdentifierNode("mytable"),
//                    SetClause = new SqlListNode<SqlInfixOperationNode>
//                    {
//                        new SqlInfixOperationNode
//                        {
//                            Left = new SqlIdentifierNode("columna"),
//                            Operator = new SqlOperatorNode("="),
//                            Right = new SqlNullNode()
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Update_SetDefault()
//        {
//            const string s = "UPDATE mytable SET columna = DEFAULT;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlUpdateNode
//                {
//                    Source = new SqlObjectIdentifierNode("mytable"),
//                    SetClause = new SqlListNode<SqlInfixOperationNode>
//                    {
//                        new SqlInfixOperationNode
//                        {
//                            Left = new SqlIdentifierNode("columna"),
//                            Operator = new SqlOperatorNode("="),
//                            Right = new SqlKeywordNode("DEFAULT")
//                        }
//                    }
//                }
//            );
//        }
//    }
//}