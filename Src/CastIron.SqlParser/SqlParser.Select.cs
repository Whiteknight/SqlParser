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
            
            selectNode.Top = ParseSelectTopClause(t);
            selectNode.Columns.AddRange(ParseSelectColumnList(t));
            selectNode.FromClause = ParseSelectFromClause(t);
            selectNode.OrderBy = ParseSelectOrderByClause(t);
            selectNode.GroupBy = ParseSelectGroupByClause(t);
            return selectNode;
        }

        private SqlSelectTopNode ParseSelectTopClause(SqlTokenizer t)
        {
            // "TOP" "(" <Number> | <Variable> ")" "PERCENT"? "WITH TIES"?
            // "TOP" <Number> | <Variable> "PERCENT"? "WITH TIES"?
            if (!t.NextIs(SqlTokenType.Keyword, "TOP"))
                return null;

            var topToken = t.GetNext();

            var numberOrVariable = ParseMaybeParenthesis(t, () => ParseNumberOrVariable(t));

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

        private IEnumerable<SqlNode> ParseSelectColumnList(SqlTokenizer t)
        {
            // "*"
            // <Column> ("," <Column>)*
            var list = new List<SqlNode>();
            while (true)
            {
                var column = ParseSelectColumn(t);
                list.Add(column);
                if (!t.NextIs(SqlTokenType.Symbol, ",", true))
                    break;
            }

            return list;
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
                    return new SqlAssignVariableNode
                    {
                        Location = equalsOperator.Location,
                        Variable = new SqlVariableNode(next),
                        RValue = ParseSelectColumnExpression(t)
                    };
                }
            }

            // TODO: <Number> | <String>

            t.PutBack(next);

            // <SelectColumnExpression> (AS <Alias>)?
            var expr = ParseSelectColumnExpression(t);
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

        private SqlNode ParseSelectColumnExpression(SqlTokenizer t)
        {
            // TODO: Need more robust expression parsing
            var n = t.GetNext();
            if (n.IsType(SqlTokenType.Identifier))
            {
                t.PutBack(n);
                return ParseSelectColumnIdentifier(t);
            }

            if (n.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(n);
            if (n.IsType(SqlTokenType.Number))
                return new SqlNumberNode(n);
            if (n.IsType(SqlTokenType.QuotedString))
                return new SqlStringNode(n);

            return null;
        }

        private SqlNode ParseSelectColumnIdentifier(SqlTokenizer t)
        {
            // <Identifier> ("." ("*" | <Identifier>))?
            var first = t.Expect(SqlTokenType.Identifier);
            if (!t.NextIs(SqlTokenType.Symbol, ".", true))
                return new SqlIdentifierNode(first);

            var second = t.GetNext();
            if (second.Is(SqlTokenType.Symbol, "*"))
            {
                return new SqlQualifiedIdentifierNode
                {
                    Location = first.Location,
                    Qualifier = new SqlIdentifierNode(first),
                    Identifier = new SqlStarNode { Location = second.Location }
                };
            }

            if (second.IsType(SqlTokenType.Identifier))
            {
                return new SqlQualifiedIdentifierNode
                {
                    Location = first.Location,
                    Qualifier = new SqlIdentifierNode(first),
                    Identifier = new SqlIdentifierNode(second)
                };
            }

            throw new Exception($"Unknown column identifier {first.Value}.{second.Value}");
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
                condition = ParseJoinOnCondition(t);

            return new SqlJoinNode
            {
                Location = tableExpression1.Location,
                Left = tableExpression1,
                Operator = join,
                Right = tableExpression2,
                OnCondition = condition
            };
        }

        private SqlNode ParseJoinOnCondition(SqlTokenizer t)
        {
            // TODO: Need real condition expression parsing
            // <column> <operator> <column>
            var on = t.Expect(SqlTokenType.Keyword, "ON");
            var left = ParseVariableOrDottedIdentifier(t);
            var op = t.Expect(SqlTokenType.Symbol);
            var right = ParseVariableOrDottedIdentifier(t);
            return new SqlInfixOperationNode
            {
                Location = on.Location,
                Left = left,
                Operator = new SqlOperatorNode { Operator = op.Value },
                Right = right
            };
        }

        private SqlOperatorNode ParseJoinOperator(SqlTokenizer t)
        {
            // (LEFT | RIGHT)? (INNER | OUTER)? JOIN
            // NATURAL? JOIN
            // CROSS APPLY
            
            var k = t.GetNext();
            if (!k.IsKeyword())
            {
                t.PutBack(k);
                return null;
            }
            if (k.Value == "CROSS")
            {
                t.Expect(SqlTokenType.Keyword, "APPLY");
                return new SqlOperatorNode("CROSS APPLY", k.Location);
            }
            if (k.Value == "NATURAL")
            {
                t.Expect(SqlTokenType.Keyword, "JOIN");
                return new SqlOperatorNode("NATURAL JOIN", k.Location);
            }

            var joinOperator = new List<SqlToken>();
            var location = k.Location;
            if (k.Value == "FULL")
            {
                joinOperator.Add(k);
                k = t.GetNext();
            }
            if (k.Value == "LEFT" || k.Value == "RIGHT")
            {
                joinOperator.Add(k);
                k = t.GetNext();
            }

            if (k.Value == "INNER" || k.Value == "OUTER")
            {
                joinOperator.Add(k);
                k = t.GetNext();
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
            var orderByNode = new SqlSelectOrderByClauseNode
            {
                Location = orderByToken.Location
            };
            while (true)
            {
                var term = ParseOrderTerm(t);
                orderByNode.Entries.Add(term);
                if (!t.NextIs(SqlTokenType.Symbol, ","))
                    break;
                t.GetNext();
            }
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

        private SqlNode ParseSelectGroupByClause(SqlTokenizer t)
        {
            if (!t.NextIs(SqlTokenType.Keyword, "GROUP"))
                return null;
            //t.GetNext();
            //t.Expect(SqlTokenType.Keyword, "BY");
            return null;
        }
    }
}
