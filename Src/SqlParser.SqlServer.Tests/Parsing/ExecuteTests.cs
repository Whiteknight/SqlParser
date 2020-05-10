using System;
using System.Linq;
using NUnit.Framework;
using SqlParser.Ast;
using SqlParser.SqlServer.Tests.Utility;
using SqlParser.SqlStandard;
using SqlParser.Tokenizing;

namespace SqlParser.SqlServer.Tests.Parsing
{
    [TestFixture]
    public class ExecuteTests
    {
        [Test]
        public void Execute_NoArgs()
        {
            const string s = "EXECUTE myProc;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlObjectIdentifierNode("myProc"),
                }
            );
        }

        [Test]
        public void Exec_NoArgs()
        {
            const string s = "EXEC myProc;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlObjectIdentifierNode("myProc"),
                }
            );
        }

        [Test]
        public void Execute_ScalarArgs()
        {
            const string s = "EXECUTE myProc 5, 'TEST';";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlObjectIdentifierNode("myProc"),
                    Arguments = new SqlListNode<SqlExecuteArgumentNode>
                    {
                        new SqlExecuteArgumentNode
                        {
                            Value = new SqlNumberNode(5)
                        },
                        new SqlExecuteArgumentNode
                        {
                            Value = new SqlStringNode("TEST")
                        }
                    }
                }
            );
        }

        [Test]
        public void Execute_NamedScalarArgs()
        {
            const string s = "EXECUTE myProc @size=5, @name='TEST';";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlObjectIdentifierNode("myProc"),
                    Arguments = new SqlListNode<SqlExecuteArgumentNode>
                    {
                        new SqlExecuteArgumentNode
                        {
                            AssignVariable = new SqlVariableNode("@size"),
                            Value = new SqlNumberNode(5)
                        },
                        new SqlExecuteArgumentNode
                        {
                            AssignVariable = new SqlVariableNode("@name"),
                            Value = new SqlStringNode("TEST")
                        }
                    }
                }
            );
        }

        [Test]
        public void Execute_OutputArgs()
        {
            const string s = "EXECUTE myProc @size OUTPUT, @count OUT;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlObjectIdentifierNode("myProc"),
                    Arguments = new SqlListNode<SqlExecuteArgumentNode>
                    {
                        new SqlExecuteArgumentNode
                        {
                            Value = new SqlVariableNode("@size"),
                            IsOut = true
                        },
                        new SqlExecuteArgumentNode
                        {
                            Value = new SqlVariableNode("@count"),
                            IsOut = true
                        }
                    }
                }
            );
        }

        [Test]
        public void Execute_StringLiteral()
        {
            const string s = "EXECUTE 'SELECT * FROM MyTable';";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlStringNode("SELECT * FROM MyTable")
                }
            );
        }

        [Test]
        public void Execute_StringExpressionTerm()
        {
            const string s = "EXECUTE ('SELECT * ' + 'FROM MyTable');";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlParenthesisNode<ISqlNode>()
                    {
                        Expression = new SqlInfixOperationNode
                        {
                            Left = new SqlStringNode("SELECT * "),
                            Operator = new SqlOperatorNode("+"),
                            Right = new SqlStringNode("FROM MyTable")

                        }
                    }
                }
            );
        }

        [Test]
        public void Execute_StringExpressionThrows()
        {
            // EXEC only takes a single term, such as a string, a variable or a parenthesized
            // expression. A non-parenthesized infix expression is going to cause an exception
            // of any variety.
            const string s = "EXECUTE 'SELECT * ' + 'FROM MyTable';";
            var target = new Parser();
            Action act  = () => target.Parse(s);
            FluentAssertions.AssertionExtensions.Should(act).Throw<Exception>();
        }

        [Test]
        public void Execute_Variable()
        {
            const string s = "EXECUTE @string;";
            var target = new Parser();
            var result = target.Parse(s);
            result.Should().PassValidation().And.RoundTrip();

            result.Statements.First().Should().MatchAst(
                new SqlExecuteNode
                {
                    Name = new SqlVariableNode("@string")
                }
            );
        }
    }
}
