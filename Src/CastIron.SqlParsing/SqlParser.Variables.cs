using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlDeclareNode ParseDeclare(SqlTokenizer t)
        {
            var declare = t.Expect(SqlTokenType.Keyword, "DECLARE");
            var v = t.Expect(SqlTokenType.Variable);
            // TODO: Improve data type parsing
            var dataType = ParseDataType(t);
            var declareNode = new SqlDeclareNode
            {
                Location = declare.Location,
                DataType = dataType,
                Variable = new SqlVariableNode(v)
            };
            if (t.NextIs(SqlTokenType.Symbol, "=", true))
                declareNode.Initializer = ParseScalarExpression(t);
            return declareNode;
        }

        private SqlNode ParseDataType(SqlTokenizer t)
        {
            var next = t.GetNext();

            // TODO: Should we add TABLE declaration parsing?
            if (next.IsKeyword("TABLE"))
                return null;

            var dataType = new SqlDataTypeNode
            {
                Location = next.Location,
                DataType = new SqlKeywordNode(next),
            };
            if (t.Peek().IsSymbol("("))
            {
                t.GetNext();
                var lookahead = t.Peek();
                if (lookahead.IsKeyword("MAX"))
                    dataType.Size = new SqlKeywordNode(t.GetNext());
                else if (lookahead.IsType(SqlTokenType.Number))
                    dataType.Size = ParseList(t, ParseNumber);
                else
                    throw new Exception($"Unexpected token {lookahead} at {lookahead.Location}");
                t.Expect(SqlTokenType.Symbol, ")");
            }
            return dataType;
        }

        private SqlSetNode ParseSet(SqlTokenizer t)
        {
            var setToken = t.Expect(SqlTokenType.Keyword, "SET");
            var v = t.Expect(SqlTokenType.Variable);
            t.Expect(SqlTokenType.Symbol, "=");
            var expr = ParseScalarExpression(t);

            return new SqlSetNode
            {
                Location = setToken.Location,
                Variable = new SqlVariableNode(v),
                Right = expr
            };
        }
    }
}
