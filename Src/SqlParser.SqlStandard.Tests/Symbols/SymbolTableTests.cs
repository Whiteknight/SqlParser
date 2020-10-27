using FluentAssertions;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Symbols;
using SqlParser.Symbols;

namespace SqlParser.SqlServer.Tests.Symbols
{
    [TestFixture]
    public class SymbolTableTests
    {
        [Test]
        public void BuildSymbolTable_TableName()
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
                FromClause = new SqlIdentifierNode { Name = "MyTable" }
            };
            ast.BuildSymbolTables();
            ast.Symbols.GetInfoOrThrow("MyTable", null).OriginKind.Should().Be(SymbolOriginKind.Environmental);
            ast.Symbols.GetInfoOrThrow("ColumnA", null).OriginKind.Should().Be(SymbolOriginKind.Environmental);
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
            ast.Symbols.GetInfoOrThrow("@var", null).OriginKind.Should().Be(SymbolOriginKind.UserDeclared);
        }
    }
}
