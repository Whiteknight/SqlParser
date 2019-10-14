using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.PostgreSql.Parsing;
using SqlParser.PostgreSql.Tests.Utility;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Tests
{
    [TestFixture]
    public class InsertValuesTests
    {
        [Test]
        public void Insert_ValuesOneRowTwoColumns()
        {
            const string s = "INSERT INTO MyTable(Column1, Column2) VALUES (1, 'TEST');";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1"),
                        new SqlIdentifierNode("Column2")
                    },
                    Source = new SqlValuesNode
                    {
                        Values = new SqlListNode<SqlListNode<ISqlNode>>
                        {
                            new SqlListNode<ISqlNode>
                            {
                                new SqlNumberNode(1),
                                new SqlStringNode("TEST")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Insert_ValuesTwoRowsTwoColumns()
        {
            const string s = "INSERT INTO MyTable(Column1, Column2) VALUES (1, 'TESTA'), (2, 'TESTB');";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            var output = result.ToString();
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1"),
                        new SqlIdentifierNode("Column2")
                    },
                    Source = new SqlValuesNode
                    {
                        Values = new SqlListNode<SqlListNode<ISqlNode>>
                        {
                            new SqlListNode<ISqlNode>
                            {
                                new SqlNumberNode(1),
                                new SqlStringNode("TESTA")
                            },
                            new SqlListNode<ISqlNode>
                            {
                                new SqlNumberNode(2),
                                new SqlStringNode("TESTB")
                            }
                        }
                    }
                }
            );
        }

        [Test]
        public void Insert_DefaultValues()
        {
            const string s = "INSERT INTO MyTable(Column1, Column2) DEFAULT VALUES;";
            var target = new Parser();
            var result = target.Parse(Tokenizer.ForPostgreSql(s));
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1"),
                        new SqlIdentifierNode("Column2")
                    },
                    Source = new SqlKeywordNode("DEFAULT VALUES")
                }
            );
        }
    }
}