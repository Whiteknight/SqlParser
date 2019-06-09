﻿using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class InsertValuesTests
    {
        [Test]
        public void Insert_ValuesOneRowTwoColumns()
        {
            const string s = "INSERT INTO MyTable(Column1, Column2) VALUES (1, 'TEST');";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            var output = result.ToString();
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1"),
                        new SqlIdentifierNode("Column2")
                    },
                    Source = new SqlInsertValuesNode
                    {
                        Values = new SqlListNode<SqlListNode<SqlNode>>
                        {
                            new SqlListNode<SqlNode>
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
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            var output = result.ToString();
            result.Should().RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlInsertNode
                {
                    Table = new SqlObjectIdentifierNode("MyTable"),
                    Columns = new SqlListNode<SqlIdentifierNode>
                    {
                        new SqlIdentifierNode("Column1"),
                        new SqlIdentifierNode("Column2")
                    },
                    Source = new SqlInsertValuesNode
                    {
                        Values = new SqlListNode<SqlListNode<SqlNode>>
                        {
                            new SqlListNode<SqlNode>
                            {
                                new SqlNumberNode(1),
                                new SqlStringNode("TESTA")
                            },
                            new SqlListNode<SqlNode>
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
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));
            result.Should().RoundTrip();

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