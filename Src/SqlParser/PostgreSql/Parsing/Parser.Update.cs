using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseUpdateStatement(ITokenizer t)
        {
            // "UPDATE" <TopClause>? "SET" <SetList> <WhereClause>?
            var updateToken = t.Expect(SqlTokenType.Keyword, "UPDATE");
            // TODO: TOP clause
            var table = ParseMaybeAliasedScalar(t, ParseVariableOrObjectIdentifier);
            var setList = ParseUpdateSetClause(t);
            // TODO: RETURNING clause
            var where = ParseWhereClause(t);
            return new SqlUpdateNode
            {
                Location = updateToken.Location,
                Source = table,
                SetClause = setList,
                WhereClause = where
            };
        }

        private SqlListNode<SqlInfixOperationNode> ParseUpdateSetClause(ITokenizer t)
        {
            t.Expect(SqlTokenType.Keyword, "SET");
            var setList = ParseList(t, ParseUpdateColumnAssignExpression);
            return setList;
        }

        private SqlInfixOperationNode ParseUpdateColumnAssignExpression(ITokenizer t)
        {
            // (<Column> | <Variable>) <CompareOp> ("DEFAULT" | <Expression>)
            var columnName = ParseVariableOrQualifiedIdentifier(t);
            // TODO: Other assignment operators 
            var opToken = t.Expect(SqlTokenType.Symbol, "=", "+=", "-=", "*=", "/=", "%=", "&=", "^=", "|=");
            ISqlNode rvalue;
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
