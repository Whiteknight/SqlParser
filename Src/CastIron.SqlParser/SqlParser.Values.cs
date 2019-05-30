using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseNumberOrVariable(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            throw new Exception($"Expecting number or variable but found {next}");
        }

        private SqlNumberNode ParseNumber(SqlTokenizer t)
        {
            var next = t.GetNext();
            if(next.IsSymbol("-"))
            {
                var x = t.Peek();
                if (x.IsType(SqlTokenType.Number))
                {
                    t.GetNext();
                    return new SqlNumberNode
                    {
                        Location = next.Location,
                        Value = decimal.Parse("-" + x.Value)
                    };
                }
            }

            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            t.PutBack(next);
            return null;
        }

        private SqlNode ParseVariableConstantOrQualifiedIdentifier(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.QuotedString))
                return new SqlStringNode(next);
            t.PutBack(next);
            var number = ParseNumber(t);
            if (number != null)
                return number;
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier != null)
                return identifier;

            throw new Exception("Cannot parse variable, constant or qualified identifier");
        }

        private SqlNode ParseVariableOrConstant(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.QuotedString))
                return new SqlStringNode(next);
            t.PutBack(next);
            var number = ParseNumber(t);
            if (number != null)
                return number;
            throw new Exception("Cannot parse variable or constant");
        }

        private SqlNode ParseVariableOrQualifiedIdentifier(SqlTokenizer t)
        {
            var next = t.GetNext();

            // <Variable>
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            t.PutBack(next);
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier != null)
                return identifier;

            throw new Exception("Cannot parse variable or identifier");
        }

        private SqlNode ParseQualifiedIdentifier(SqlTokenizer t)
        {
            // ( <Qualifier> "." )? <Identifier>
            var next = t.Peek();
            if (!next.IsType(SqlTokenType.Identifier))
                return null;
            t.GetNext();
            if (!t.Peek().Is(SqlTokenType.Symbol, "."))
                return new SqlIdentifierNode(next);
            t.GetNext();
            var qualifier = new SqlIdentifierNode(next);
            var identifier = t.GetNext();
            return new SqlQualifiedIdentifierNode
            {
                Location = qualifier.Location,
                Qualifier = qualifier,
                Identifier = identifier.Is(SqlTokenType.Symbol, "*") ? (SqlNode)new SqlStarNode() : new SqlIdentifierNode(identifier)
            };
        }
    }
}
