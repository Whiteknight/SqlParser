﻿using System.Collections.Generic;
using System.Linq;
using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseQueryExpression(Tokenizer t)
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

        private ISqlNode ParseQuerySpecificiation(Tokenizer t)
        {
            // "SELECT" ...
            var selectToken = t.Expect(SqlTokenType.Keyword, "SELECT");
            var selectNode = new SqlSelectNode
            {
                Location = selectToken.Location
            };

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
            selectNode.GroupByClause = ParseSelectGroupByClause(t);
            selectNode.HavingClause = ParseSelectHavingClause(t);
            selectNode.OrderByClause = ParseSelectOrderByClause(t);
            // TODO: MySql-style LIMIT clause
            return selectNode;
        }

        private ISqlNode ParseWhereClause(Tokenizer t)
        {
            // "WHERE" <BooleanExpression>
            if (!t.NextIs(SqlTokenType.Keyword, "WHERE"))
                return null;

            t.GetNext();
            return ParseBooleanExpression(t);
        }

        private ISqlNode ParseSelectHavingClause(Tokenizer t)
        {
            // "HAVING" <BooleanExpression>
            if (!t.NextIs(SqlTokenType.Keyword, "HAVING"))
                return null;

            t.GetNext();
            return ParseBooleanExpression(t);
        }

        private SqlSelectTopNode ParseSelectTopClause(Tokenizer t)
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

        private ISqlNode ParseSelectColumn(Tokenizer t)
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
            return ParseMaybeAliasedScalar(t, ParseSelectColumnExpression);
        }

        private ISqlNode ParseSelectColumnExpression(Tokenizer t)
        {
            var expr = ParseScalarExpression(t);
            var lookahead = t.Peek();
            if (lookahead.IsKeyword("OVER"))
            {
                // "OVER" "(" <OverPartitionBy>? <OverOrderBy>? <OverRows>? ")"
                var overToken = t.GetNext();
                t.Expect(SqlTokenType.Symbol, "(");
                var overNode = new SqlOverNode
                {
                    Location = overToken.Location,
                    Expression = expr,
                    PartitionBy = ParseOverPartitionBy(t),
                    OrderBy = ParseOverOrderBy(t),
                    RowsRange = ParseOverRows(t)
                };
                t.Expect(SqlTokenType.Symbol, ")");
                return overNode;
            }

            return expr;
        }

        private ISqlNode ParseOverPartitionBy(Tokenizer t)
        {
            // "PARTITION" "BY" <Expr> ("," <Expr>)* 
            if (!t.NextIs(SqlTokenType.Keyword, "PARTITION", true))
                return null;
            t.Expect(SqlTokenType.Keyword, "BY");
            return ParseList(t, ParseScalarExpression);
        }

        private ISqlNode ParseOverOrderBy(Tokenizer t)
        {
            // "ORDER" "BY" <OrderByTerm> ("," <OrderByTerm>)*
            if (!t.NextIs(SqlTokenType.Keyword, "ORDER", true))
                return null;
            t.Expect(SqlTokenType.Keyword, "BY");
            return ParseList(t, ParseOrderTerm);
        }

        private ISqlNode ParseOverRows(Tokenizer t)
        {
            var lookahead = t.Peek();
            if (!lookahead.IsKeyword("ROWS") && !lookahead.IsKeyword("RANGE"))
                return null;
            // TODO: this
            return null;
        }

        private ISqlNode ParseSelectFromClause(Tokenizer t)
        {
            // ("FROM" <join>)?
            if (!t.NextIs(SqlTokenType.Keyword, "FROM"))
                return null;
            t.GetNext();
            return ParseJoin(t);
        }

        private ISqlNode ParseJoin(Tokenizer t)
        {
            // <TableExpression> (<JoinOperator> <TableExpression> "ON" <JoinCondition>)?
            // TODO: <TableExpression> ("WITH" <Hint>)?
            var tableExpression1 = ParseMaybeAliasedTable(t, ParseTableOrSubexpression);

            var join = ParseJoinOperator(t);
            if (join == null)
                return tableExpression1;
            var tableExpression2 = ParseMaybeAliasedTable(t, ParseTableOrSubexpression);

            ISqlNode condition = null;
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

        private SqlOperatorNode ParseJoinOperator(Tokenizer t)
        {
            // "CROSS" ("APPLY" | "JOIN")
            // "NATURAL" "JOIN"
            // "INNER" "JOIN"
            // ("LEFT" | "RIGHT")?  "OUTER"?  "JOIN"
            
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
                throw ParsingException.CouldNotParseRule(nameof(ParseJoinOperator), k);
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

            // TODO: hints: "MERGE" | "HASH" | "REDISTRIBUTE" | "REPLICATE" | "REDUCE"

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

        private ISqlNode ParseTableOrSubexpression(Tokenizer t)
        {
            // <ObjectIdentifier> | <tableVariable> | "(" <QueryExpression> ")" | "(" <ValuesExpression> ")"
            var lookahead = t.Peek();

            // <ObjectIdentifier>
            if (lookahead.IsType(SqlTokenType.Identifier))
                return ParseObjectIdentifier(t);

            // <tableVariable>
            if (lookahead.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(t.GetNext());

            // "(" <Subexpression> ")"
            if (lookahead.Is(SqlTokenType.Symbol, "("))
                return ParseParenthesis(t, ParseSubexpression);

            throw ParsingException.CouldNotParseRule(nameof(ParseTableOrSubexpression), lookahead);
        }

        private ISqlNode ParseSubexpression(Tokenizer t)
        {
            // <QueryExpresion> | <Values>
            var lookahead = t.Peek();
            ISqlNode expr = null;
            if (lookahead.IsKeyword("SELECT"))
                expr = ParseQueryExpression(t);
            else if (lookahead.IsKeyword("VALUES"))
                expr = ParseValues(t);
            if (expr != null)
                return expr;

            throw ParsingException.CouldNotParseRule(nameof(ParseSubexpression), lookahead);
        }

        private SqlSelectOrderByClauseNode ParseSelectOrderByClause(Tokenizer t)
        {
            // "ORDER" "BY" <OrderTerm>+ ("OFFSET" <NumberOrVariable> "ROWS")? ("FETCH" "NEXT" <NumberOrVariable> "ROWS" "ONLY")?
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
                t.Expect(SqlTokenType.Keyword, "ROWS"); // TODO: Can also be "ROW"
            }
            if (t.NextIs(SqlTokenType.Keyword, "FETCH"))
            {
                t.GetNext();
                t.Expect(SqlTokenType.Keyword, "NEXT"); // TODO: can also be "FIRST"
                orderByNode.Limit = ParseNumberOrVariable(t);
                t.Expect(SqlTokenType.Keyword, "ROWS"); // TODO: Can also be "ROW"
                t.Expect(SqlTokenType.Keyword, "ONLY");
            }

            return orderByNode;
        }

        private SqlOrderByEntryNode ParseOrderTerm(Tokenizer t)
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

        private ISqlNode ParseSelectGroupByClause(Tokenizer t)
        {
            // "GROUP" "BY" <IdentifierList>
            if (!t.NextIs(SqlTokenType.Keyword, "GROUP"))
                return null;
            t.GetNext();
            t.Expect(SqlTokenType.Keyword, "BY");

            return ParseList(t, ParseQualifiedIdentifier);
        }
    }
}
