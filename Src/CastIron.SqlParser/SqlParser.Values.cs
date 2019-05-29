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

        private SqlNode ParseVariableOrDottedIdentifier(SqlTokenizer t)
        {
            var next = t.GetNext();

            // <Variable>
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            t.PutBack(next);
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier != null)
                return identifier;

            throw new Exception("Cannot parse identifier");
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
            var identifier = t.Expect(SqlTokenType.Identifier);
            return new SqlQualifiedIdentifierNode
            {
                Location = qualifier.Location,
                Qualifier = qualifier,
                Identifier = new SqlIdentifierNode(identifier)
            };
        }
    }
}
