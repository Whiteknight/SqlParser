﻿using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseCaseExpression(SqlTokenizer t)
        {
            var caseToken = t.Expect(SqlTokenType.Keyword, "CASE");
            var caseNode = new SqlCaseNode
            {
                Location = caseToken.Location,
                InputExpression = ParseScalarExpression(t)
            };
            while(true)
            {
                var lookahead = t.Peek();
                if (lookahead.IsKeyword("END"))
                {
                    t.GetNext();
                    return caseNode;
                }
                if (lookahead.IsKeyword("ELSE"))
                {
                    t.GetNext();
                    caseNode.ElseExpression = ParseScalarExpression(t);
                    t.Expect(SqlTokenType.Keyword, "END");
                    return caseNode;
                }
                if (lookahead.IsKeyword("WHEN"))
                {
                    var whenNode = t.GetNext();
                    var condition = ParseScalarExpression(t);
                    t.Expect(SqlTokenType.Keyword, "THEN");
                    var result = ParseScalarExpression(t);
                    caseNode.WhenExpressions.Add(new SqlCaseWhenNode
                    {
                        Location = whenNode.Location,
                        Condition = condition,
                        Result = result
                    });
                    continue;
                }

                throw new Exception($"Unexpected token {lookahead} in CASE statement at {lookahead.Location}");
            }
        }

        private SqlNode ParseScalarExpression(SqlTokenizer t)
        {
            return ParseScalarExpression4(t);
        }

        private SqlNode ParseScalarExpression4(SqlTokenizer t)
        {
            // <CaseExpression> | <Expression3>
            if (t.Peek().IsKeyword("CASE"))
                return ParseCaseExpression(t);

            return ParseScalarExpression3(t);
        }

        private SqlNode ParseScalarExpression3(SqlTokenizer t)
        {
            // Operators with + - precidence
            // <Expression2> (<op> <Expression2>)+
            var left = ParseScalarExpression2(t);
            while (t.Peek().IsSymbol("+", "-", "&", "^", "|"))
            {
                var op = new SqlOperatorNode(t.GetNext());
                var right = ParseScalarExpression2(t);
                left = new SqlInfixOperationNode
                {
                    Location = op.Location,
                    Left = left,
                    Operator = op,
                    Right = right
                };
            }

            return left;
        }

        private SqlNode ParseScalarExpression2(SqlTokenizer t)
        {
            // Operators with * / % precidence
            // <Expression1> (<op> <Expression1>)+
            var left = ParseScalarExpression1(t);
            while (t.Peek().IsSymbol("*", "/", "%"))
            {
                var op = new SqlOperatorNode(t.GetNext());
                var right = ParseScalarExpression1(t);
                left = new SqlInfixOperationNode
                {
                    Location = op.Location,
                    Left = left,
                    Operator = op,
                    Right = right
                };
            }

            return left;
        }

        private SqlNode ParseScalarExpression1(SqlTokenizer t)
        {
            // "NULL" | ("-" | "+" | "~") <Expression0> | <Expression0>
            var next = t.Peek();
            if (next.IsKeyword("NULL"))
            {
                var nullToken = t.GetNext();
                return new SqlNullNode(nullToken);
            }
            if (next.IsSymbol("-", "+", "~"))
            {
                var op = t.GetNext();
                var expr = ParseScalarExpression1(t);
                return new SqlPrefixOperationNode
                {
                    Location = op.Location,
                    Operator = new SqlOperatorNode(op),
                    Right = expr
                };
            }

            return ParseScalarExpression0(t);
        }

        private SqlNode ParseScalarExpression0(SqlTokenizer t)
        {
            // Terminal expression
            // <MethodCall> | <Identifier> | <Variable> | <String> | <Number> | "(" <Expression> ")"
            var next = t.Peek();
            if (next.IsType(SqlTokenType.Identifier))
            {
                var name = t.GetNext();
                if (t.Peek().IsSymbol("("))
                {
                    t.PutBack(name);
                    return ParseFunctionCall(t);
                }

                t.PutBack(name);
                return ParseQualifiedIdentifier(t);
            }
            // Some of the built-in function names are treated as Keyword by the tokenizer, so 
            // try to account for those here
            if (next.IsType(SqlTokenType.Keyword))
            {
                var name = t.GetNext();
                if (t.Peek().IsSymbol("("))
                {
                    t.PutBack(name);
                    return ParseFunctionCall(t);
                }

                t.PutBack(name);
            }

            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(t.GetNext());
            if (next.IsType(SqlTokenType.QuotedString))
                return new SqlStringNode(t.GetNext());
            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(t.GetNext());

            if (next.IsSymbol("("))
            {
                t.GetNext();
                var value = ParseScalarExpression(t);
                t.Expect(SqlTokenType.Symbol, ")");
                return value;
            }

            throw new Exception($"Error parsing expression. Unexpected token {next} at {next.Location}");
        }

        private SqlNode ParseFunctionCall(SqlTokenizer t)
        {
            var name = t.GetNext();
            if (name.Type != SqlTokenType.Keyword && name.Type != SqlTokenType.Identifier)
                throw new Exception($"Expecting function name but found {name}");
            var arguments = ParseParenthesis(t, x => ParseList(x, ParseScalarExpression)).Expression;
            return new SqlFunctionCallNode
            {
                Location = name.Location,
                Name = new SqlIdentifierNode(name),
                Arguments = arguments 
            };
        }
    }
}