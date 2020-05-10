//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Symbols;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Symbols;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class SetTests
//    {
//        [Test]
//        public void Set_Number()
//        {
//            const string s = "var := 5;";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSetNode
//                {
//                    Variable = new SqlIdentifierNode("var"),
//                    Operator = new SqlOperatorNode(":="),
//                    Right = new SqlNumberNode(5)
//                }
//            );
//        }

//        [Test]
//        public void Set_SelectExpression()
//        {
//            const string s = "var := (SELECT 5);";
//            var target = new Parser();
//            var result = target.Parse(s);
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSetNode
//                {
//                    Variable = new SqlIdentifierNode("var"),
//                    Operator = new SqlOperatorNode(":="),
//                    Right = new SqlParenthesisNode<ISqlNode>
//                    {
//                        Expression = new SqlSelectNode
//                        {
//                            Columns = new SqlListNode<ISqlNode>
//                            {
//                                new SqlNumberNode(5)
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Set_CompoundOperator()
//        {
//            // Postgres doesn't support compound assignment operators, but does attempt to translate them
//            // in the stringifier. Parse T-SQL with a compound operator, serialize to PSQL, and parse that
//            // to prove that the conversion works as expected
//            const string s = "SET @var += 5;";
//            var target = new SqlStandard.Parser();
//            var result1 = target.Parse(s);
//            result1.BuildSymbolTables();
//            //new[] { new SymbolInfo { OriginalName = "var", OriginKind = SymbolOriginKind.UserDeclared } }
//            var psql = result1.ToPostgreSqlString();
//            var result2 = new Parser().Parse(psql);
//            // We need to build symbol tables to make sure we can properly translate "@var" to "var"
            
//            result2.Statements.First().Should().MatchAst(
//                new SqlSetNode
//                {
//                    Variable = new SqlIdentifierNode("var"),
//                    Operator = new SqlOperatorNode(":="),
//                    Right = new SqlInfixOperationNode {
//                        Left = new SqlIdentifierNode("var"),
//                        Operator = new SqlOperatorNode("+"),
//                        Right = new SqlNumberNode(5)
//                    }
//                }
//            );
//        }
//    }
//}