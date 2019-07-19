using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser
{
    public partial class Parser
    {
        private SqlNode ParseCaseExpression(Tokenizer t)
        {
            // "CASE" <Expression> <When>+ <Else>? "END"
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

                throw ParsingException.CouldNotParseRule(nameof(ParseCaseExpression), lookahead);
            }
        }

        private SqlNode ParseScalarExpression(Tokenizer t)
        {
            // Top-level general-purpose expression parsing method, redirects to the appropriate
            // precidence level
            return ParseScalarExpression4(t);
        }

        private SqlNode ParseScalarTerm(Tokenizer t)
        {
            // Parses only a single term
            return ParseScalarExpression0(t);
        }

        private SqlNode ParseScalarExpression4(Tokenizer t)
        {
            // <CaseExpression> | <Expression3>
            if (t.Peek().IsKeyword("CASE"))
                return ParseCaseExpression(t);

            return ParseScalarExpression3(t);
        }

        private SqlNode ParseScalarExpression3(Tokenizer t)
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

        private SqlNode ParseScalarExpression2(Tokenizer t)
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

        private SqlNode ParseScalarExpression1(Tokenizer t)
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

        private SqlNode ParseScalarExpression0(Tokenizer t)
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
                // "(" (<QueryExpression> | <ScalarExpression>) ")"
                // e.g. SET @x = (select 5) or INSERT INTO x(num) VALUES (1, 2, (select 3))
                var value = ParseParenthesis(t, x =>
                {
                    if (x.Peek().IsKeyword("SELECT"))
                        return ParseQueryExpression(t);
                    return ParseScalarExpression(t);
                });
                if (value.Expression is SqlSelectNode)
                    return value;
                return value.Expression;
            }

            throw ParsingException.CouldNotParseRule(nameof(ParseScalarExpression0), next);
        }

        private SqlNode ParseFunctionCall(Tokenizer t)
        {
            var name = t.GetNext();
            if (name.Type != SqlTokenType.Keyword && name.Type != SqlTokenType.Identifier)
                throw ParsingException.UnexpectedToken(SqlTokenType.Identifier, name);

            // "COUNT" "(" "*" ")"
            if (name.IsKeyword("COUNT"))
            {
                var openParen = t.Expect(SqlTokenType.Symbol, "(");
                var maybeStar = t.Peek();
                if (maybeStar.IsSymbol("*"))
                {
                    t.GetNext();
                    t.Expect(SqlTokenType.Symbol, ")");
                    return new SqlFunctionCallNode
                    {
                        Location = name.Location,
                        Name = new SqlKeywordNode(name),
                        Arguments = new SqlListNode<SqlNode> { new SqlOperatorNode(maybeStar)}
                    };
                }

                // It's not *, so put everything back, fallthrough, and let the rest of the parsing happen
                t.PutBack(openParen);
            }

            // TODO: "CONVERT" "(" <DataType>, <ScalarExpression> ("," <int>)? ")"
            // "CAST" "(" <ScalarExpression> "AS" <DataType> ")"
            if (name.IsKeyword("CAST"))
            {
                t.Expect(SqlTokenType.Symbol, "(");
                var first = ParseScalarExpression(t);
                t.Expect(SqlTokenType.Keyword, "AS");
                var type = ParseDataType(t);
                t.Expect(SqlTokenType.Symbol, ")");
                return new SqlCastNode
                {
                    Location = name.Location,
                    Expression = first,
                    DataType = type
                };
            }

            // <Name> "(" <ScalarExpressionList> ")"
            return new SqlFunctionCallNode
            {
                Location = name.Location,
                Name = new SqlIdentifierNode(name),
                Arguments = ParseParenthesis(t, x => ParseList(x, ParseScalarExpression)).Expression
            };
        }
    }
}
