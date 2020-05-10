//using System.Linq;
//using NUnit.Framework;
//using SqlParser.Ast;
//using SqlParser.PostgreSql.Parsing;
//using SqlParser.PostgreSql.Tests.Utility;
//using SqlParser.Tokenizing;

//namespace SqlParser.PostgreSql.Tests
//{
//    [TestFixture]
//    public class SelectOverTests
//    {
//        [Test]
//        public void Select_RowNumberOverPartition()
//        {
//            const string s = "SELECT ROW_NUMBER() OVER (PARTITION BY columna)";
//            var target = new Parser();
//            var result = target.Parse(Tokenizer.ForPostgreSql(s));
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOverNode
//                        {
//                            Expression = new SqlFunctionCallNode
//                            {
//                                Name = new SqlIdentifierNode("row_number")
//                            },
//                            PartitionBy = new SqlListNode<ISqlNode>
//                            {
//                                new SqlIdentifierNode("columna")
//                            }
//                        }
//                    }
//                }
//            );
//        }

//        [Test]
//        public void Select_AvgOverOrderBy()
//        {
//            const string s = "SELECT AVG(columna) OVER (ORDER BY columna) FROM mytable";
//            var target = new Parser();
//            var result = target.Parse(Tokenizer.ForPostgreSql(s));
//            result.Should().PassValidation().And.RoundTrip();

//            result.Statements.First().Should().MatchAst(
//                new SqlSelectNode
//                {
//                    Columns = new SqlListNode<ISqlNode>
//                    {
//                        new SqlOverNode
//                        {
//                            Expression = new SqlFunctionCallNode
//                            {
//                                Name = new SqlIdentifierNode("avg"),
//                                Arguments = new SqlListNode<ISqlNode>
//                                {
//                                    new SqlIdentifierNode("columna")
//                                }
//                            },
//                            OrderBy = new SqlListNode<SqlOrderByEntryNode>
//                            {
//                                new SqlOrderByEntryNode
//                                {
//                                    Source = new SqlIdentifierNode("columna")
//                                }
//                            }
//                        }
//                    },
//                    FromClause = new SqlObjectIdentifierNode("mytable")
//                }
//            );
//        }
//    }
//}