using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private SqlDeclareNode ParseDeclare(ITokenizer t)
        {
            // "DECLARE" <variable> <DataType> ("=" <Expression>)?
            // TODO: "DECLARE" <variable> <DataType> ("=" <Expression>)? ("," <variable> <DataType> ("=" <Expression>)?)*
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

        private SqlDataTypeNode ParseDataType(ITokenizer t)
        {
            // <Keyword> ("(" ("MAX" | <SizeList>)? ")")?
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
                    throw ParsingException.CouldNotParseRule(nameof(ParseDataType), lookahead);
                t.Expect(SqlTokenType.Symbol, ")");
            }
            return dataType;
        }

        private SqlSetNode ParseSet(ITokenizer t)
        {
            // "SET" <variable> <assignOp> <Expression>
            var setToken = t.Expect(SqlTokenType.Keyword, "SET");
            var v = t.Expect(SqlTokenType.Variable);
            var op = t.Expect(SqlTokenType.Symbol, "=", "+=", "-=", "*=", "/=", "%=", "&=", "^=", "|=");
            var expr = ParseScalarExpression(t);

            return new SqlSetNode
            {
                Location = setToken.Location,
                Variable = new SqlVariableNode(v),
                Operator = new SqlOperatorNode(op),
                Right = expr
            };
        }
    }
}
