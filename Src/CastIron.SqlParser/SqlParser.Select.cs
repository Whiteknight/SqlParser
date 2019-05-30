using System;
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

            return new SqlUnionStatementNode
            {
                Location = firstQuery.Location,
                First = firstQuery,
                Operator = t.NextIs(SqlTokenType.Keyword, "ALL", true) ? "UNION ALL" : unionOperator.Value,
                Second = ParseQueryExpression(t)
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
            selectNode.WhereClause = ParseSelectWhereClause(t);
            selectNode.OrderByClause = ParseSelectOrderByClause(t);
            selectNode.GroupByClause = ParseSelectGroupByClause(t);
            selectNode.HavingClause = ParseSelectHavingClause(t);
            return selectNode;
        }

        private SqlSelectWhereClauseNode ParseSelectWhereClause(SqlTokenizer t)
        {
            if (!t.NextIs(SqlTokenType.Keyword, "WHERE"))
                return null;

            var whereToken = t.GetNext();
            var expression = ParseLogicalExpression(t);
            return new SqlSelectWhereClauseNode
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
            var expression = ParseLogicalExpression(t);
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
                return new SqlStarNode { Location = next.Location };

            // TODO: Should change this to (<Variable> "=")? <SelectColumnExpression>
            // <Variable> ("=" <SelectColumnExpression>)? 
            if (next.IsType(SqlTokenType.Variable))
            {
                if (t.NextIs(SqlTokenType.Symbol, "="))
                {
                    var equalsOperator = t.GetNext();
                    var rvalue = ParseScalarExpression(t);
                    return new SqlAssignVariableNode
                    {
                        Location = equalsOperator.Location,
                        Variable = new SqlVariableNode(next),
                        RValue = rvalue
                    };
                }
            }

            t.PutBack(next);

            // <SelectColumnExpression> (AS <Alias>)?
            var expr = ParseScalarExpression(t);
            if (t.NextIs(SqlTokenType.Keyword, "AS"))
            {
                var op = t.GetNext();
                var alias = t.Expect(SqlTokenType.Identifier);
                return new SqlAliasNode
                {
                    Location = op.Location,
                    Source = expr,
                    Alias = new SqlIdentifierNode(alias)
                };
            }

            return expr;
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
            var tableExpression1 = ParseTableExpression(t);

            var join = ParseJoinOperator(t);
            if (join == null)
                return tableExpression1;
            var tableExpression2 = ParseTableExpression(t);

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
                throw new Exception("Unknown JOIN operator");

            t.PutBack(k);
            return null;
        }

        private SqlNode ParseTableExpression(SqlTokenizer t)
        {
            // <TableOrSubexpression> ("AS"? <Alias>)?
            var tableOrSubexpr = ParseTableOrSubexpression(t);
            
            var next = t.Peek();
            if (next.IsKeyword("AS"))
            {
                var a = t.GetNext();
                var alias = t.Expect(SqlTokenType.Identifier);
                return new SqlAliasNode
                {
                    Location = a.Location,
                    Source = tableOrSubexpr,
                    Alias = new SqlIdentifierNode(alias)
                };
            }
            if (next.IsType(SqlTokenType.Identifier))
            {
                t.GetNext();
                return new SqlAliasNode
                {
                    Location = tableOrSubexpr.Location,
                    Source = tableOrSubexpr,
                    Alias = new SqlIdentifierNode(next)
                };
            }

            return tableOrSubexpr;
        }

        private SqlNode ParseTableOrSubexpression(SqlTokenizer t)
        {
            // TODO: select * from (VALUES (1), (2), (3)) x(id)
            // <QualifiedIdentifier> | <tableVariable> | "(" <Subexpression> ")"

            var qualifiedIdentifier = ParseQualifiedIdentifier(t);
            if (qualifiedIdentifier != null)
                return qualifiedIdentifier;

            var next = t.GetNext();

            // <tableVariable>

            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);

            // "(" <Subexpression> ")"
            if (next.Is(SqlTokenType.Symbol, "("))
            {
                var expr = ParseQueryExpression(t);
                var subexpression = new SqlSelectSubexpressionNode
                {
                    Location = expr.Location,
                    Select = expr
                };
                t.Expect(SqlTokenType.Symbol, ")");
                return subexpression;
            }

            throw new Exception($"Unexpected token {next} when parsing table name");
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
                throw new Exception("Expected identifier or number");
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
                Location = groupByToken.Location
            };
            groupByNode.Keys = ParseList(t, ParseQualifiedIdentifier);
            return groupByNode;
        }
    }
}
