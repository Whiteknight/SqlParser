using System;
using FluentAssertions;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Symbols;
using SqlParser.Symbols;

namespace SqlParser.SqlServer.Tests.Symbols
{
    [TestFixture]
    public class SymbolTableErrorsTests
    {
        [Test]
        public void BuildSymbolTable_TableNameNotDefined()
        {
            var ast = new SqlSelectNode
            {
                Columns = new SqlListNode<ISqlNode>
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
                Columns = new SqlListNode<ISqlNode>
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