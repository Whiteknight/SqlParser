using System.Collections.Generic;
using System.Linq;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseQueryExpression(SqlTokenizer t)
        {
            // <querySpecification> ( <UnionOperator> <querySpecification> )*
            var firstQuery = ParseQuerySpecificiation(t);
            var unionOperator = t.Peek();
            if (!unionOperator.IsKeyword("UNION") && !unionOperator.IsKeyword("EXCEPT") && !unionOperator.IsKeyword("INTERSECT"))
                return firstQuery;

            unionOperator = t.GetNext();

            return new SqlInfixOperationNode
            {
                Location = firstQuery.Location,
                Left = firstQuery,
                Operator = new SqlOperatorNode(t.NextIs(SqlTokenType.Keyword, "ALL", true) ? "UNION ALL" : unionOperator.Value),
                Right = ParseQueryExpression(t)
            };
        }

        private SqlNode ParseQuerySpecificiation(SqlTokenizer t)
        {
            // "SELECT" ...
            var selectToken = t.Expect(SqlTokenType.Keyword, "SELECT");
            var selectNode = new SqlSelectNode();
            selectNode.Location = selectToken.Location;

            var modifier = t.Peek();
            if (modifier.IsKeyword("ALL") || modifier.IsKeyword("DISTINCT"))
            {
                t.GetNext();
                selectNode.Modifier = modifier.Value;
            }
            
            selectNode.TopClause = ParseSelectTopClause(t);
            selectNode.Columns = ParseList(t, ParseSelectColumn);
            selectNode.FromClause = ParseSelectFromClause(t);
            selectNode.WhereClause = ParseWhereClause(t);
            selectNode.OrderByClause = ParseSelectOrderByClause(t);
            selectNode.GroupByClause = ParseSelectGroupByClause(t);
            selectNode.HavingClause = ParseSelectHavingClause(t);
            return selectNode;
        }

        private SqlWhereNode ParseWhereClause(SqlTokenizer t)
        {
            if (!t.NextIs(SqlTokenType.Keyword, "WHERE"))
                return null;

            var whereToken = t.GetNext();
            var expression = ParseBooleanExpression(t);
            return new SqlWhereNode
            {
                Location = whereToken.Location,
                SearchCondition = expression
            };
        }

        private SqlSelectHavingClauseNode ParseSelectHavingClause(SqlTokenizer t)
        {
            if (!t.NextIs(SqlTokenType.Keyword, "HAVING"))
                return null;

            var whereToken = t.GetNext();
            var expression = ParseBooleanExpression(t);
            return new SqlSelectHavingClauseNode
            {
                Location = whereToken.Location,
                SearchCondition = expression
            };
        }

        private SqlSelectTopNode ParseSelectTopClause(SqlTokenizer t)
        {
            // "TOP" "(" <Number> | <Variable> ")" "PERCENT"? "WITH TIES"?
            // "TOP" <Number> | <Variable> "PERCENT"? "WITH TIES"?
            if (!t.NextIs(SqlTokenType.Keyword, "TOP"))
                return null;

            var topToken = t.GetNext();

            var numberOrVariable = ParseMaybeParenthesis(t, ParseNumberOrVariable);

            bool percent = t.NextIs(SqlTokenType.Keyword, "PERCENT", true);
            bool withTies = false;
            if (t.NextIs(SqlTokenType.Keyword, "WITH", true))
            {
                t.Expect(SqlTokenType.Keyword, "TIES");
                withTies = true;
            }

            return new SqlSelectTopNode {
                Location = topToken.Location,
                Value = numberOrVariable,
                Percent = percent,
                WithTies = withTies
            };
        }

        private SqlNode ParseSelectColumn(SqlTokenizer t)
        {
            var next = t.GetNext();

            // "*"
            if (next.Is(SqlTokenType.Symbol, "*"))
                return new SqlOperatorNode(next);

            // TODO: Should change this to (<Variable> "=")? <SelectColumnExpression>
            // <Variable> ("=" <SelectColumnExpression>)? 
            if (next.IsType(SqlTokenType.Variable))
            {
                if (t.NextIs(SqlTokenType.Symbol, "="))
                {
                    var equalsOperator = t.GetNext();
                    var rvalue = ParseScalarExpression(t);
                    return new SqlInfixOperationNode
                    {
                        Location = equalsOperator.Location,
                        Left = new SqlVariableNode(next),
                        Right = rvalue,
                        Operator = new SqlOperatorNode(equalsOperator)
                    };
                }
            }

            t.PutBack(next);

            // <SelectColumnExpression> (AS <Alias>)?
            return ParseMaybeAliased(t, ParseScalarExpression);
        }

        private SqlSelectFromClauseNode ParseSelectFromClause(SqlTokenizer t)
        {
            // (FROM <join>)?
            if (!t.NextIs(SqlTokenType.Keyword, "FROM"))
                return null;
            var from = t.GetNext();
            var source = ParseJoin(t);
            return new SqlSelectFromClauseNode
            {
                Location = from.Location,
                Source = source
            };
        }

        private SqlNode ParseJoin(SqlTokenizer t)
        {
            // <TableExpression> (<JoinOperator> <TableExpression> "ON" <JoinCondition>)?
            // TODO: <TableExpression> ("WITH" <Hint>)?
            var tableExpression1 = ParseMaybeAliased(t, ParseTableOrSubexpression);

            var join = ParseJoinOperator(t);
            if (join == null)
                return tableExpression1;
            var tableExpression2 = ParseMaybeAliased(t, ParseTableOrSubexpression);

            SqlNode condition = null;
            if (join.Operator != "NATURAL JOIN")
            {
                // "ON" <BooleanExpression>
                t.Expect(SqlTokenType.Keyword, "ON");
                condition = ParseBooleanExpression(t);
            }

            return new SqlJoinNode
            {
                Location = tableExpression1.Location,
                Left = tableExpression1,
                Operator = join,
                Right = tableExpression2,
                OnCondition = condition
            };
        }

        private SqlOperatorNode ParseJoinOperator(SqlTokenizer t)
        {
            // "CROSS" ("APPLY" | "JOIN")
            //  "NATURAL" "JOIN"
            // "INNER" "JOIN"
            // ("LEFT" | "RIGHT")?  "OUTER"? "JOIN"
            
            var k = t.GetNext();
            if (!k.IsKeyword())
            {
                t.PutBack(k);
                return null;
            }
            if (k.Value == "CROSS")
            {
                if (t.NextIs(SqlTokenType.Keyword, "APPLY", true))
                    return new SqlOperatorNode("CROSS APPLY", k.Location);
                if (t.NextIs(SqlTokenType.Keyword, "JOIN", true))
                    return new SqlOperatorNode("CROSS JOIN", k.Location);
            }
            if (k.Value == "NATURAL")
            {
                t.Expect(SqlTokenType.Keyword, "JOIN");
                return new SqlOperatorNode("NATURAL JOIN", k.Location);
            }
            if (k.Value == "INNER")
            {
                t.Expect(SqlTokenType.Keyword, "JOIN");
                return new SqlOperatorNode("INNER JOIN", k.Location);

            }

            var joinOperator = new List<SqlToken>();
            var location = k.Location;
            if (k.Value == "FULL" || k.Value == "LEFT" || k.Value == "RIGHT")
            {
                joinOperator.Add(k);
                k = t.GetNext();
            }

            if (k.Value == "OUTER")
            {
                joinOperator.Add(k);
                k = t.GetNext();
                if (k.Value == "APPLY")
                    return new SqlOperatorNode("OUTER APPLY");
            }

            if (k.Value == "JOIN")
            {
                joinOperator.Add(k);
                var op = string.Join(" ", joinOperator.Select(j => j.Value));
                return new SqlOperatorNode(op, location);
            }

            if (joinOperator.Count > 0)
                throw ParsingException.CouldNotParseRule(nameof(ParseJoinOperator), k);

            t.PutBack(k);
            return null;
        }

        private SqlNode ParseTableOrSubexpression(SqlTokenizer t)
        {
            // TODO: select * from (VALUES (1), (2), (3)) x(id)
            // <QualifiedIdentifier> | <tableVariable> | "(" <Subexpression> ")"

            if (t.Peek().IsType(SqlTokenType.Identifier))
                return ParseObjectIdentifier(t);

            var next = t.GetNext();

            // <tableVariable>

            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);

            // "(" <Subexpression> ")"
            if (next.Is(SqlTokenType.Symbol, "("))
            {
                var expr = ParseQueryExpression(t);
                var subexpression = new SqlParenthesisNode<SqlNode>
                {
                    Location = expr.Location,
                    Expression = expr
                };
                t.Expect(SqlTokenType.Symbol, ")");
                return subexpression;
            }

            throw ParsingException.CouldNotParseRule(nameof(ParseTableOrSubexpression), next);
        }

        private SqlSelectOrderByClauseNode ParseSelectOrderByClause(SqlTokenizer t)
        {
            if (!t.NextIs(SqlTokenType.Keyword, "ORDER"))
                return null;

            var orderByToken = t.GetNext();
            t.Expect(SqlTokenType.Keyword, "BY");
            var orderByItems = ParseList(t, ParseOrderTerm);
            var orderByNode = new SqlSelectOrderByClauseNode
            {
                Location = orderByToken.Location,
                Entries = orderByItems

            };
            if (t.NextIs(SqlTokenType.Keyword, "OFFSET"))
            {
                t.GetNext();
                orderByNode.Offset = ParseNumberOrVariable(t);
                t.Expect(SqlTokenType.Keyword, "ROWS");
            }
            if (t.NextIs(SqlTokenType.Keyword, "FETCH"))
            {
                t.GetNext();
                t.Expect(SqlTokenType.Keyword, "NEXT");
                orderByNode.Limit = ParseNumberOrVariable(t);
                t.Expect(SqlTokenType.Keyword, "ROWS");
                t.Expect(SqlTokenType.Keyword, "ONLY");
            }

            return orderByNode;
        }

        private SqlOrderByEntryNode ParseOrderTerm(SqlTokenizer t)
        {
            // ( <QualifiedIdentifier> | <Number> ) ("ASC" | "DESC")?
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier == null)
            {
                if (t.Peek().IsType(SqlTokenType.Number))
                {
                    var number = t.GetNext();
                    identifier = new SqlNumberNode(number);
                }
            }

            if (identifier == null)
                throw ParsingException.CouldNotParseRule(nameof(ParseOrderTerm), t.Peek());
            var entry = new SqlOrderByEntryNode
            {
                Location = identifier.Location,
                Source = identifier
            };
            var next = t.Peek();
            if (next.IsKeyword("ASC") || next.IsKeyword("DESC"))
            {
                t.GetNext();
                entry.Direction = next.Value;
            }

            return entry;
        }

        private SqlSelectGroupByNode ParseSelectGroupByClause(SqlTokenizer t)
        {
            if (!t.NextIs(SqlTokenType.Keyword, "GROUP"))
                return null;
            var groupByToken = t.GetNext();
            t.Expect(SqlTokenType.Keyword, "BY");

            var groupByNode = new SqlSelectGroupByNode
            {
                Location = groupByToken.Location,
                Keys = ParseList(t, ParseQualifiedIdentifier)
            };
            return groupByNode;
        }
    }
}
