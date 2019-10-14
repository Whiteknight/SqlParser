using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        public ISqlNode ParseExecute(Tokenizer t)
        {
            // ("EXEC"|"EXECUTE") ( <stringExpression> | <objectIdentifier> <ListOfArgument> )
            // TODO: @return_status = ...
            // TODO: WITH <execute_option>
            var execToken = t.Expect(SqlTokenType.Keyword, "EXEC", "EXECUTE");
            var execNode = new SqlExecuteNode()
            {
                Location = execToken.Location,
            };
            if (t.Peek().IsType(SqlTokenType.Identifier))
            {
                execNode.Name = ParseObjectIdentifier(t);
                execNode.Arguments = ParseList(t, ParseExecArgument);
                return execNode;
            }

            bool isParenthesis = t.Peek().IsSymbol("(");
            var stringExpression = ParseScalarTerm(t);
            if (isParenthesis)
                stringExpression = new SqlParenthesisNode<ISqlNode>(stringExpression);
            execNode.Name = stringExpression;
            return execNode;
        }

        public SqlExecuteArgumentNode ParseExecArgument(Tokenizer t)
        {
            // (<parameter> "=")? (<Expression> | "DEFAULT" | <Variable> ("OUT"|"OUTPUT"))
            var next = t.GetNext();
            if (next.IsSymbol(";") || next.IsType(SqlTokenType.EndOfInput))
                return null;
            if (next.IsKeyword("DEFAULT"))
            {
                var keyword = new SqlKeywordNode(next);
                return new SqlExecuteArgumentNode
                {
                    Location = keyword.Location,
                    Value = keyword
                };
            }

            if (next.IsType(SqlTokenType.Variable))
            {
                var lookahead = t.Peek();
                if (lookahead.IsSymbol("="))
                {
                    t.GetNext();
                    return new SqlExecuteArgumentNode
                    {
                        Location = next.Location,
                        AssignVariable = new SqlVariableNode(next),
                        Value = ParseScalarExpression(t),
                        IsOut = t.NextIs(SqlTokenType.Keyword, "OUT", true) || t.NextIs(SqlTokenType.Keyword, "OUTPUT", true)
                    };
                }
            }

            ISqlNode expression = null;
            if (next.IsType(SqlTokenType.Identifier, SqlTokenType.Number, SqlTokenType.QuotedString, SqlTokenType.Variable))
            {
                t.PutBack(next);
                expression = ParseScalarExpression(t);
            }

            else if (next.IsType(SqlTokenType.Keyword))
            {
                if (t.Peek().IsSymbol("("))
                {
                    t.PutBack(next);
                    expression = ParseScalarExpression(t);
                }
            }

            if (expression == null)
            {
                t.PutBack(next);
                return null;
            }

            return new SqlExecuteArgumentNode
            {
                Location = next.Location,
                Value = expression,
                IsOut = t.NextIs(SqlTokenType.Keyword, "OUT", true) || t.NextIs(SqlTokenType.Keyword, "OUTPUT", true)
            };
        }
    }
}
