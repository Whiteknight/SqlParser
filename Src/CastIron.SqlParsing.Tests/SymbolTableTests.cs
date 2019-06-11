using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Symbols;
using FluentAssertions;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class SymbolTableTests
    {
        [Test]
        public void BuildSymbolTable_TableName()
        {
            var ast = new SqlSelectNode
            {
                Columns = new SqlListNode<SqlNode>
                {
                    new SqlQualifiedIdentifierNode
                    {
                        Qualifier = new SqlIdentifierNode("MyTable"),
                        Identifier = new SqlIdentifierNode("ColumnA")
                    }
                },
                FromClause = new SqlIdentifierNode { Name = "MyTable" }
            };
            ast.BuildSymbolTables();
            ast.Symbols.GetInfoOrThrow("MyTable").DataType.Should().Be("TableExpression");
            ast.Symbols.GetInfoOrThrow("ColumnA").DataType.Should().Be("Column");
        }

        [Test]
        public void BuildSymbolTable_TableNameNotDefined()
        {
            var ast = new SqlSelectNode
            {
                Columns = new SqlListNode<SqlNode>
                {
                    new SqlQualifiedIdentifierNode
                    {
                        Qualifier = new SqlIdentifierNode("MyTable2"),
                        Identifier = new SqlIdentifierNode("ColumnA")
                    }
                },
                FromClause = new SqlIdentifierNode { Name = "MyTable1" }
            };
            Action act = () => ast.BuildSymbolTables();
            act.Should().Throw<SymbolNotDefinedException>().And.Symbol.Should().Be("MyTable2");
        }

        [Test]
        public void BuildSymbolTable_TableNameAlreadyDefined()
        {
            var ast = new SqlSelectNode
            {
                Columns = new SqlListNode<SqlNode>
                {
                    new SqlQualifiedIdentifierNode
                    {
                        Qualifier = new SqlIdentifierNode("MyTable"),
                        Identifier = new SqlIdentifierNode("ColumnA")
                    }
                },
                FromClause = new SqlJoinNode
                {
                    Left = new SqlIdentifierNode { Name = "MyTable" },
                    Operator = new SqlOperatorNode("NATURAL JOIN"),
                    Right = new SqlIdentifierNode { Name = "MyTable" }
                }
            };
            Action act = () => ast.BuildSymbolTables();
            act.Should().Throw<SymbolAlreadyDefinedException>().And.Symbol.Should().Be("MyTable");
        }

        [Test]
        public void BuildSymbolTable_DeclareSet()
        {
            var ast = new SqlStatementListNode
            {
                new SqlDeclareNode
                {
                    Variable = new SqlVariableNode("@var"),
                    DataType = new SqlKeywordNode("INT")
                },
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Right = new SqlNumberNode(5)
                }
            };
            ast.BuildSymbolTables();
            ast.Symbols.GetInfoOrThrow("@var").DataType.Should().Be("Variable");
        }

        [Test]
        public void BuildSymbolTable_SetWithoutDeclare()
        {
            var ast = new SqlStatementListNode
            {
                new SqlSetNode
                {
                    Variable = new SqlVariableNode("@var"),
                    Right = new SqlNumberNode(5)
                }
            };
            Action act = () => ast.BuildSymbolTables();
            act.Should().Throw<SymbolNotDefinedException>().And.Symbol.Should().Be("@var");
        }
    }
}
