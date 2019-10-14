using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseBooleanExpression(Tokenizer t)
        {
            return ParseBooleanExpression5(t);
        }

        private ISqlNode ParseBooleanExpression5(Tokenizer t)
        {
            // <BooleanExpression3> ("AND" | "OR") <BooleanExpression3>
            var left = ParseBooleanExpression4(t);
            while (t.Peek().IsKeyword("AND", "OR"))
            {
                var op = new SqlOperatorNode(t.GetNext());
                var right = ParseBooleanExpression4(t);
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

        private ISqlNode ParseBooleanExpression4(Tokenizer t)
        {
            // "NOT"? <BooleanExpression4>
            if (t.Peek().IsKeyword("NOT"))
            {
                var notToken = t.GetNext();
                var right = ParseBooleanExpression3(t);
                return new SqlPrefixOperationNode
                {
                    Operator = new SqlOperatorNode(notToken),
                    Location = notToken.Location,
                    Right = right
                };
            }

            return ParseBooleanExpression3(t);
        }

        private ISqlNode ParseBooleanExpression3(Tokenizer t)
        {
            // "(" <BooleanExpression> ")
            // <BooleanExpression2>
            var lookahead = t.Peek();
            if (lookahead.IsSymbol("("))
            {
                t.GetNext();
                var value = ParseBooleanExpression(t);
                t.Expect(SqlTokenType.Symbol, ")");
                return value;
            }

            return ParseBooleanExpression2(t);
        }

        private ISqlNode ParseBooleanExpression2(Tokenizer t)
        {
            // "EXISTS" "(" <QueryExpression> ")"
            // <BooleanExpression1>
            var lookahead = t.Peek();
            if (lookahead.Is(SqlTokenType.Keyword, "EXISTS"))
            {
                t.GetNext();
                var query = ParseParenthesis(t, ParseQueryExpression);
                return new SqlPrefixOperationNode
                {
                    Location = lookahead.Location,
                    Operator = new SqlOperatorNode("EXISTS"),
                    Right = query
                };
            }

            return ParseBooleanExpression1(t);
        }

        private ISqlNode ParseBooleanExpression1(Tokenizer t)
        {
            // <ScalarExpression> <ComparisonOperator> <ScalarExpression>
            // <ScalarExpression> "IS" "NOT"? "NULL"
            // <ScalarExpression> "NOT"? "BETWEEN" <ScalarExpression> "AND" <ScalarExpression>
            // <ScalarExpression> "NOT"? "IN" "(" <ValueList> ")"
            // <ScalarExpression> "NOT"? "LIKE" <String>
            var left = ParseScalarExpression(t);
            var operatorToken = t.Peek();
            if (operatorToken.IsSymbol(">", "<", "=", "<=", ">=", "!=", "<>"))
            {
                t.GetNext();
                if (t.Peek().IsKeyword("ALL", "ANY", "SOME"))
                {
                    var prefix = t.GetNext();
                    var query = ParseParenthesis(t, ParseQueryExpression);
                    return new SqlInfixOperationNode
                    {
                        Location = operatorToken.Location,
                        Left = left,
                        Right = query,
                        Operator = new SqlOperatorNode(operatorToken.Value + " " + prefix.Value)
                    };
                }
                var right = ParseScalarExpression(t);
                return new SqlInfixOperationNode
                {
                    Location = operatorToken.Location,
                    Left = left,
                    Right = right,
                    Operator = new SqlOperatorNode(operatorToken)
                };
            }
            if (operatorToken.IsKeyword("IS"))
            {
                t.GetNext();
                var not = t.NextIs(SqlTokenType.Keyword, "NOT", true);
                var nullToken = t.Expect(SqlTokenType.Keyword, "NULL");
                return new SqlInfixOperationNode
                {
                    Location = operatorToken.Location,
                    Left = left,
                    Operator = new SqlOperatorNode
                    {
                        Location = operatorToken.Location,
                        Operator = not ? "IS NOT" : "IS"
                    },
                    Right = new SqlNullNode(nullToken)
                };
            }

            var isNot = false;
            if (operatorToken.IsKeyword("NOT"))
            {
                t.GetNext();
                isNot = true;
                operatorToken = t.Peek();
            }

            if (operatorToken.IsKeyword("BETWEEN"))
            {
                t.GetNext();
                var first = ParseScalarExpression(t);
                t.Expect(SqlTokenType.Keyword, "AND");
                var second = ParseScalarExpression(t);
                return new SqlBetweenOperationNode
                {
                    Location = operatorToken.Location,
                    Not = isNot,
                    Left = left,
                    Low = first,
                    High = second
                };
            }
            if (operatorToken.IsKeyword("IN"))
            {
                t.GetNext();
                // TODO: "IN" "(" <QueryExpression> ")"
                var list = ParseParenthesis(t, t2 => ParseList(t2, ParseVariableOrConstant)).Expression;
                return new SqlInNode
                {
                    Not = isNot,
                    Search = left,
                    Location = operatorToken.Location,
                    Items = list
                };
            }
            if (operatorToken.IsKeyword("LIKE"))
            {
                t.GetNext();
                var right = ParseMaybeParenthesis(t, ParseString);
                return new SqlInfixOperationNode
                {
                    Left = left,
                    Operator = new SqlOperatorNode
                    {
                        Location = operatorToken.Location,
                        Operator = (isNot ? "NOT " : "") + "LIKE"
                    },
                    Right = right
                };
            }

            // There's no operator, it's just a scalar expression in a boolean context.
            // This happens in CASE statements, for example. Return the scalar expression
            // as-is and let the AST deal with it.
            return left;
        }
    }
}
