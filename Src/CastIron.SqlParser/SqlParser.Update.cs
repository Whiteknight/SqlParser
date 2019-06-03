using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseUpdateStatement(SqlTokenizer t)
        {
            // "UPDATE" <TopClause>? "SET" <SetList> <WhereClause>?
            var updateToken = t.Expect(SqlTokenType.Keyword, "UPDATE");
            // TODO: TOP clause
            var table = ParseMaybeAliased(t, ParseVariableOrObjectIdentifier);
            t.Expect(SqlTokenType.Keyword, "SET");
            var setList = ParseList(t, ParseUpdateColumnAssignExpression);
            // TODO: OUTPUT clause
            var where = ParseWhereClause(t);
            return new SqlUpdateNode
            {
                Location = updateToken.Location,
                Source = table,
                SetClause = setList,
                WhereClause = where
            };
        }

        private SqlInfixOperationNode ParseUpdateColumnAssignExpression(SqlTokenizer t)
        {
            // (<Column> | <Variable>) <CompareOp> ("DEFAULT" | <Expression>)
            var columnName = ParseVariableOrQualifiedIdentifier(t);
            // TODO: Other assignment operators 
            var opToken = t.Expect(SqlTokenType.Symbol, "=", "+=", "-=", "*=", "/=", "%=", "&=", "^=", "|=");
            SqlNode rvalue;
            var lookahead = t.Peek();
            if (lookahead.Is(SqlTokenType.Keyword, "DEFAULT"))
            {
                t.GetNext();
                rvalue = new SqlKeywordNode(lookahead);
            }
            else
                rvalue = ParseScalarExpression(t);
            return new SqlInfixOperationNode
            {
                Left = columnName,
                Location = columnName.Location,
                Operator = new SqlOperatorNode(opToken),
                Right = rvalue
            };
        }
    }
}
