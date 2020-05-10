//using System.Collections.Generic;
//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Tokenizing;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class UnionTests
//    {
//        [TestCase("UNION")]
//        [TestCase("UNION ALL")]
//        [TestCase("EXCEPT")]
//        [TestCase("INTERSECT")]
//        public void Select_UnionOperatorSelect(string op)
//        {
//            string s = $"SELECT * FROM table1 {op} SELECT * FROM table2";
//            var target = new Parser();
//            var result = target.Parse(Tokenizer.ForPostgreSql(s));
//            result.Should().PassValidation().And.RoundTrip();
//            var o1 = result.ToString();

//            result.Statements.First().Should().MatchAst(
//                new SqlInfixOperationNode
//                {
//                    Left = new SqlSelectNode
//                    {
//                        Columns = new SqlListNode<ISqlNode>
//                        {
//                            Children = new List<ISqlNode>
//                            {
//                                new SqlOperatorNode("*")
//                            }
//                        },
//                        FromClause = new SqlObjectIdentifierNode("table1")
//                    },
//                    Operator = new SqlOperatorNode(op),
//                    Right = new SqlSelectNode
//                    {
//                        Columns = new SqlListNode<ISqlNode>
//                        {
//                            Children = new List<ISqlNode>
//                            {
//                                new SqlOperatorNode("*")
//                            }
//                        },
//                        FromClause = new SqlObjectIdentifierNode("table2")
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_UnionSelect2x()
//        {
//            const string s = "SELECT * FROM table1 UNION SELECT * FROM table2 UNION ALL SELECT * FROM table3;";
//            var target = new Parser();
//            var result = target.Parse(Tokenizer.ForPostgreSql(s));
//            var output = result.ToString();
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlInfixOperationNode
//                {
//                    Left = new SqlSelectNode
//                    {
//                        Columns = new SqlListNode<ISqlNode>
//                        {
//                            Children = new List<ISqlNode>
//                            {
//                                new SqlOperatorNode("*")
//                            }
//                        },
//                        FromClause = new SqlObjectIdentifierNode("table1")
//                    },
//                    Operator = new SqlOperatorNode("UNION"),
//                    Right = new SqlInfixOperationNode
//                    {
//                        Left = new SqlSelectNode
//                        {
//                            Columns = new SqlListNode<ISqlNode>
//                            {
//                                Children = new List<ISqlNode>
//                                {
//                                    new SqlOperatorNode("*")
//                                }
//                            },
//                            FromClause = new SqlObjectIdentifierNode("table2")
//                        },
//                        Operator = new SqlOperatorNode("UNION ALL"),
//                        Right = new SqlSelectNode
//                        {
//                            Columns = new SqlListNode<ISqlNode>
//                            {
//                                Children = new List<ISqlNode>
//                                {
//                                    new SqlOperatorNode("*")
//                                }
//                            },
//                            FromClause = new SqlObjectIdentifierNode("table3")
//                        }
//                    }
//                }
//            );
//        }
//    }
//}