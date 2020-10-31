using ParserObjects;
using ParserObjects.Parsers;
using SqlParser.Ast;
using SqlParser.Parsing;
using SqlParser.Tokenizing;
using static ParserObjects.Parsers.ParserMethods<SqlParser.Tokenizing.SqlToken>;
using static SqlParser.Parsing.ParserMethods;

namespace SqlParser.SqlServer.Parsing
{
    public partial class Parser
    {
        private static IParser<SqlToken, ISqlNode> InitializeParser()
        {
            var parser = new SqlStandard.SqlStandardGrammar().Parser;

            var variable = parser.FindNamed("variable") as IParser<SqlToken, SqlVariableNode>;

            var number = parser.FindNamed("number") as IParser<SqlToken, SqlNumberNode>;

            var variableOrNumber = First<ISqlNode>(
                variable,
                number
            ).Named("variableOrNumber");

            var selectTopClause = Rule(
                Keyword("TOP"),
                First(
                    variableOrNumber.MaybeParenthesized(),
                    ErrorNode<SqlNumberNode>("Expecting variable or number")
                ),
                Keyword("PERCENT").Optional(),
                // TODO: If we see WITH we must require TIES
                Keyword("WITH", "TIES").Optional(),
                (top, expr, percent, withTies) => new SqlTopLimitNode
                {
                    Location = top.Location,
                    Value = expr,
                    Percent = percent != null,
                    WithTies = withTies != null
                }
            ).Optional();

            parser.Replace("selectTopClause", selectTopClause);

            return parser;
        }
    }
}
