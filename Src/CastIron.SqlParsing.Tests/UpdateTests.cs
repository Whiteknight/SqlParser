using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tests.Utility;
using CastIron.SqlParsing.Tokenizing;
using NUnit.Framework;

namespace CastIron.SqlParsing.Tests
{
    [TestFixture]
    public class UpdateTests
    {
        [Test]
        public void Update_SetWhere()
        {
            const string s = "UPDATE MyTable SET ColumnA = 1 WHERE ColumnB = 2;";
            var target = new SqlParser();
            var result = target.Parse(new SqlTokenizer(s));

            result.Statements.First().Should().MatchAst(
                new SqlUpdateNode
                {
                    Source = new SqlObjectIdentifierNode("MyTable"),
                    SetClause = new SqlListNode<SqlNode>
                    {
                        new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("ColumnA"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(1)
                        }
                    },
                    WhereClause = new SqlWhereNode
                    {
                        SearchCondition = new SqlInfixOperationNode
                        {
                            Left = new SqlIdentifierNode("ColumnB"),
                            Operator = new SqlOperatorNode("="),
                            Right = new SqlNumberNode(2)
                        }
                    }
                }
            );
        }
    }
}