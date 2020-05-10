//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Tokenizing;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class DeleteTests
//    {
//        [Test]
//        public void Delete_FromTable()
//        {
//            const string s = "DELETE FROM \"MyTable\";";
//            var target = new Parser();
//            var result = target.Parse(Tokenizer.ForPostgreSql(s));
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlDeleteNode
//                {
//                    Source = new SqlObjectIdentifierNode("MyTable")
//                }
//            );
//        }

//        [Test]
//        public void Delete_FromTableWhereCondition()
//        {
//            const string s = "DELETE FROM MyTable WHERE ColumnA = 1;";
//            var target = new Parser();
//            var result = target.Parse(Tokenizer.ForPostgreSql(s));
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlDeleteNode
//                {
//                    Source = new SqlObjectIdentifierNode("mytable"),
//                    WhereClause = new SqlInfixOperationNode
//                    {
//                        Left = new SqlIdentifierNode("columna"),
//                        Operator = new SqlOperatorNode("="),
//                        Right = new SqlNumberNode(1)
//                    }
//                }
//            );
//        }
//    }
//}
