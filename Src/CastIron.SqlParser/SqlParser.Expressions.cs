using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseLogicalExpression(SqlTokenizer t)
        {
            // <BooleanExpression> <BooleanOperator> <BooleanExpression>
            var left = ParseBooleanExpression(t);
            var operatorToken = t.Peek();
            if (!operatorToken.IsKeyword("AND", "OR"))
                return left;
            t.GetNext();
            var right = ParseLogicalExpression(t);
            return new SqlInfixOperationNode
            {
                Left = left,
                Right = right,
                Location = operatorToken.Location,
                Operator = new SqlOperatorNode(operatorToken)
            };
        }

        private SqlNode ParseUpdateColumnAssignExpression(SqlTokenizer t)
        {
            var columnName = ParseQualifiedIdentifier(t);
            t.Expect(SqlTokenType.Symbol, "=");
            var rvalue = ParseScalarExpression(t);
            return new SqlInfixOperationNode
            {
                Left = columnName,
                Location = columnName.Location,
                Operator = new SqlOperatorNode("="),
                Right = rvalue
            };
        }

        private SqlNode ParseBooleanExpression(SqlTokenizer t)
        {
            // <ScalarExpression> <ComparisonOperator> <ScalarExpression>
            // <ScalarExpression> "BETWEEN" <ScalarExpression> "AND" <ScalarExpression>
            // <ScalarExpression> "IN" "(" <ValueList> ")"
            // <ScalarExpression> "IS" "NOT"? "NULL"
            // <ScalarExpression> "LIKE" <String>
            var left = ParseScalarExpression(t);
            var operatorToken = t.Peek();
            if (operatorToken.IsSymbol(">", "<", "=", "<=", ">=", "!=", "<>"))
            {
                t.GetNext();
                var right = ParseScalarExpression(t);
                return new SqlInfixOperationNode
                {
                    Location = operatorToken.Location,
                    Left = left,
                    Right = right,
                    Operator = new SqlOperatorNode(operatorToken)
                };
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
                    Left = left,
                    Low = first,
                    High = second
                };
            }
            if (operatorToken.IsKeyword("IN"))
            {
                t.GetNext();
                var list = ParseParenthesis(t, t2 => ParseList(t2, ParseVariableOrConstant));
                return new SqlInNode
                {
                    Search = left,
                    Location = operatorToken.Location,
                    Items = list
                };
            }

            // TODO: "IS" "NOT"? "NULL"
            // TODO: "LIKE" ( "(" <String> ")" ) | <String>

            throw new Exception($"Could not parse comparison with operator {operatorToken}");
        }

        private SqlNode ParseScalarExpression(SqlTokenizer t)
        {
            return ParseScalarExpression2(t);
        }

        private SqlNode ParseScalarExpression2(SqlTokenizer t)
        {
            // TODO: Make sure that we handle negative numbers on both sides of the operator
            var left = ParseScalarExpression1(t);
            var operatorToken = t.Peek();
            if (operatorToken.IsSymbol("+", "-"))
            {
                t.GetNext();
                var right = ParseScalarExpression(t);
                return new SqlInfixOperationNode
                {
                    Location = operatorToken.Location,
                    Left = left,
                    Right = right,
                    Operator = new SqlOperatorNode(operatorToken)
                };
            }

            return left;
        }

        private SqlNode ParseScalarExpression1(SqlTokenizer t)
        {
            // TODO: Make sure that we handle negative numbers on both sides of the operator
            var left = ParseVariableConstantOrQualifiedIdentifier(t);
            var operatorToken = t.Peek();
            if (operatorToken.IsSymbol("/", "*", "%"))
            {
                t.GetNext();
                var right = ParseScalarExpression(t);
                return new SqlInfixOperationNode
                {
                    Location = operatorToken.Location,
                    Left = left,
                    Right = right,
                    Operator = new SqlOperatorNode(operatorToken)
                };
            }

            return left;
        }
    }
}
