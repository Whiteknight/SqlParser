using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private SqlDeclareNode ParseDeclare(ITokenizer t)
        {
            // "DECLARE" <identifier> <DataType> (":=" <Expression>)?
            // TODO: "DECLARE" <identifier> <DataType> (":=" <Expression>)? ("," <identifier> <DataType> (":=" <Expression>)?)*
            var declare = t.Expect(SqlTokenType.Keyword, "DECLARE");
            var v = t.Expect(SqlTokenType.Identifier);
            // TODO: Improve data type parsing
            var dataType = ParseDataType(t);
            var declareNode = new SqlDeclareNode
            {
                Location = declare.Location,
                DataType = dataType,
                Variable = new SqlIdentifierNode(v)
            };
            if (t.NextIs(SqlTokenType.Symbol, ":=", true))
                declareNode.Initializer = ParseScalarExpression(t);
            return declareNode;
        }

        private SqlDataTypeNode ParseDataType(ITokenizer t)
        {
            // <Keyword> ("(" ("MAX" | <SizeList>)? ")")?
            var next = t.GetNext();

            // TODO: Array types
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
            // <variable> ":=" <Expression>
            var v = t.GetNext();
            if (v.Type != SqlTokenType.Identifier && v.Type != SqlTokenType.Variable)
                throw ParsingException.CouldNotParseRule(nameof(ParseSet), v);
            var op = t.Expect(SqlTokenType.Symbol, ":=");
            var expr = ParseScalarExpression(t);

            return new SqlSetNode
            {
                Location = v.Location,
                Variable = new SqlIdentifierNode(v),
                Operator = new SqlOperatorNode(op),
                Right = expr
            };
        }
    }
}
