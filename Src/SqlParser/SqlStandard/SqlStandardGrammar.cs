using System.Linq;
using ParserObjects;
using ParserObjects.Parsers;
using SqlParser.Ast;
using SqlParser.Tokenizing;
using static ParserObjects.Parsers.ParserMethods;
using static SqlParser.SqlStandard.ParserMethods;

namespace SqlParser.SqlStandard
{
    public class SqlStandardGrammar
    {
        private readonly IParser<SqlToken, ISqlNode> _parser;

        public SqlStandardGrammar()
        {
            _parser = InitializeParser();
        }

        public IParser<SqlToken, ISqlNode> GetParser() => _parser;

        private IParser<SqlToken, ISqlNode> InitializeParser()
        {
            var identifierToken = Token(SqlTokenType.Identifier);
            var identifier = identifierToken.Transform(t => new SqlIdentifierNode(t));
            var keywordToken = Token(SqlTokenType.Keyword);
            var keyword = keywordToken.Transform(k => new SqlKeywordNode(k));
            var dot = Token(SqlTokenType.Symbol, ".");
            var star = Operator("*");
            var comma = Token(SqlTokenType.Symbol, ",");
            var identifierOrKeywordAsIdentifier = First(identifierToken, keywordToken).Transform(t => new SqlIdentifierNode(t));
            var number = Token(SqlTokenType.Number).Transform(t => new SqlNumberNode(t));
            var openParen = Token(SqlTokenType.Symbol, "(");
            var closeParen = Token(SqlTokenType.Symbol, ")");
            var quotedString = Token(SqlTokenType.QuotedString).Transform(t => new SqlStringNode(t));
            var variable = Token(SqlTokenType.Variable).Transform(t => new SqlVariableNode(t));
            var constant = First<SqlToken, ISqlNode>(
                quotedString,
                number
            );
            var variableOrConstant = First(
                variable,
                constant
            );
            var queryExpressionInternal = Empty<SqlToken>().Transform(x => (ISqlNode) null);
            var queryExpression = Deferred(() => queryExpressionInternal);

            var booleanExpressionInternal = Empty<SqlToken>().Transform(x => (ISqlNode) null);
            var booleanExpression = Deferred(() => booleanExpressionInternal);
            var scalarExpressionInternal = Empty<SqlToken>().Transform(x => (ISqlNode)null);
            var scalarExpression = Deferred(() => scalarExpressionInternal);

            var qualifierDotIdentifier = Rule(
                identifierOrKeywordAsIdentifier,
                dot,
                First<SqlToken, ISqlNode>(
                    star,
                    identifierOrKeywordAsIdentifier
                ),
                (qualifier, d, name) => (ISqlNode)new SqlQualifiedIdentifierNode
                {
                    Qualifier = qualifier,
                    Identifier = name,
                    Location = qualifier.Location
                }
            );
            var qualifiedIdentifier = First(
                qualifierDotIdentifier,
                identifierOrKeywordAsIdentifier
            );

            var countStarFunctionCall = Rule(
                Keyword("COUNT"),
                openParen,
                star,
                closeParen,
                (c, o, s, x) => new SqlFunctionCallNode
                {
                    Location = c.Location,
                    Name = c,
                    Arguments = new SqlListNode<ISqlNode> { s }
                }
            );

            var dataTypeSize = Parenthesized(
                First<SqlToken, ISqlNode>(
                    Keyword("MAX"),
                    number.ListSeparatedBy(comma, true).Transform(n => new SqlListNode<SqlNumberNode>(n.ToList()))
                )
            );

            var dataType = Rule(
                Match<SqlToken>(t => t.IsKeyword()).Transform(k => new SqlKeywordNode(k)),
                dataTypeSize.Optional(),
                (n, s) => new SqlDataTypeNode {
                    Location = n.Location,
                    DataType = n,
                    Size = s
                }
            );

            var castFunctionCall = Rule(
                Keyword("CAST"),
                openParen,
                scalarExpression,
                Keyword("AS"),
                dataType,
                closeParen,
                (k, o, expr, a, type, c) => new SqlCastNode
                {
                    Location = k.Location,
                    Expression = expr,
                    DataType = type
                }
            );

            var functionCall = First<SqlToken, ISqlNode>(
                countStarFunctionCall,
                castFunctionCall,
                Rule(
                    First<SqlToken, ISqlNode>(
                        keyword,
                        identifier
                    ),
                    scalarExpression.List().Transform(args => new SqlListNode<ISqlNode>(args.ToList())).Parenthesized(),
                    (name, args) => new SqlFunctionCallNode
                    {
                        Location = name.Location,
                        Name = name,
                        Arguments = args?.Expression
                    }
                )
            );

            var nullTerm = Match<SqlToken>(t => t.IsKeyword("NULL")).Transform(t => new SqlNullNode(t));

            var scalarTerm = First(
                nullTerm,
                variable,
                quotedString,
                number,
                functionCall,
                qualifiedIdentifier,
                queryExpression.Parenthesized(),
                scalarExpression.Parenthesized()
            );
                
            // TODO: Make this rule property recursive (right-associative)
            var arithmeticPrefix = First<SqlToken, ISqlNode>(
                Rule(
                    Match<SqlToken>(t => t.IsSymbol("-", "+", "~")).Transform(t => new SqlOperatorNode(t)),
                    scalarTerm,
                    (op, expr) => new SqlPrefixOperationNode {
                        Location = op.Location,
                        Operator = op,
                        Right = expr
                    }
                ),
                scalarTerm
            );

            var multiplicative = LeftApply(
                arithmeticPrefix,
                left => Rule(
                    left,
                    Match<SqlToken>(t => t.IsSymbol("*", "/", "%")).Transform(t => new SqlOperatorNode(t)),
                    arithmeticPrefix,
                    (l, op, r) => new SqlInfixOperationNode
                    {
                        Location = l.Location,
                        Left = l,
                        Operator = op,
                        Right = r
                    }
                )
            );

            var additive = LeftApply(
                multiplicative,
                left => Rule(
                    left,
                    Match<SqlToken>(t => t.IsSymbol("+", "-", "&", "^", "|")).Transform(t => new SqlOperatorNode(t)),
                    multiplicative,
                    (l, op, r) => new SqlInfixOperationNode
                    {
                        Location = l.Location,
                        Left = l,
                        Operator = op,
                        Right = r
                    }
                )
            );

            var caseWhen = Rule(
                Keyword("WHEN"),
                booleanExpression,
                Keyword("THEN"),
                scalarExpression,
                (when, cond, then, expr) => new SqlCaseWhenNode
                {
                    Location = when.Location,
                    Condition = cond,
                    Result = expr
                }
            );

            var caseElse = Rule(
                Keyword("ELSE"),
                scalarExpression,
                (e, expr) => expr
            );

            var end = Keyword("END");

            var caseBlock = Rule(
                Keyword("CASE"),
                scalarExpression,
                caseWhen.List(),
                caseElse.Optional(),
                end,
                (c, expr, when, e, x) => new SqlCaseNode {
                    Location = c.Location,
                    InputExpression = expr,
                    WhenExpressions = when.ToList(),
                    ElseExpression = e
                }
            );

            var caseExpression = First(
                caseBlock,
                additive
            );

            scalarExpressionInternal = caseExpression;

            var booleanComparison = Match<SqlToken>(t => t.IsSymbol(">", "<", "=", "<=", ">=", "!=", "<>")).Transform(t => new SqlOperatorNode(t));
            var booleanComparisonModifier = First(
                Keyword("ALL"),
                Keyword("ANY"),
                Keyword("SOME")
            );

            var booleanTerm = LeftApply(
                scalarExpression,
                left => First<SqlToken, ISqlNode>(
                    Rule(
                        left,
                        booleanComparison,
                        booleanComparisonModifier,
                        queryExpression,
                        (l, comp, mod, query) => new SqlInfixOperationNode
                        {
                            Location = l.Location,
                            Left = l,
                            Right = query,
                            Operator = new SqlOperatorNode($"{comp.Operator} {mod.Keyword}", comp.Location)
                        }
                    ),
                    Rule(
                        left,
                        Keyword("IS"),
                        Keyword("NOT").Optional(),
                        nullTerm,
                        (l, i, not, n) => new SqlInfixOperationNode
                        {
                            Location = i.Location,
                            Left = l,
                            Operator = new SqlOperatorNode
                            {
                                Location = i.Location,
                                Operator = not == null ? "IS" : "IS NOT"
                            },
                            Right = n
                        }
                    ),
                    Rule(
                        left,
                        Keyword("NOT").Optional(),
                        Keyword("BETWEEN"),
                        scalarExpression,
                        Keyword("AND"),
                        scalarExpression,
                        (l, not, between, expr1, and, expr2) => new SqlBetweenOperationNode
                        {
                            Location = between.Location,
                            Not = not != null,
                            Left = l,
                            Low = expr1,
                            High = expr2
                        }
                    ),
                    Rule(
                        left,
                        Keyword("NOT").Optional(), 
                        Keyword("IN"),
                        variableOrConstant.ListSeparatedBy(comma, true).Transform(l => new SqlListNode<ISqlNode>(l.ToList())).Parenthesized(),
                        (l, not, @in, values) => new SqlInNode
                        {
                            Not = not != null,
                            Search = l,
                            Location = @in.Location,
                            Items = values.Expression
                        }
                    ),
                    Rule(
                        left,
                        Keyword("NOT").Optional(),
                        Keyword("LIKE"),
                        quotedString.MaybeParenthesized(),
                        (l, not, like, pattern) => new SqlOperatorNode
                        {
                            Location = like.Location,
                            Operator = (not != null ? "NOT " : "") + "LIKE"
                        }
                    )
                )
            );

            var existsExpression = Rule(
                Keyword("EXISTS"),
                queryExpression.Parenthesized(),
                (kw, query) => new SqlPrefixOperationNode
                {
                    Location = kw.Location,
                    Operator = new SqlOperatorNode(kw.Keyword, kw.Location),
                    Right = query
                }
            );

            var booleanExists = First(
                existsExpression,
                booleanTerm
            ).MaybeParenthesized();

            var invertedBoolean = Rule(
                Keyword("NOT").Optional(),
                existsExpression,
                (not, expr) => not == null
                    ? expr
                    : new SqlPrefixOperationNode
                    {
                        Operator = new SqlOperatorNode(not.Keyword, not.Location),
                        Location = not.Location,
                        Right = expr
                    }
            );

            var combinedBoolean = LeftApply<SqlToken, ISqlNode>(
                invertedBoolean,
                left =>
                    Rule(
                        left,
                        Match<SqlToken>(t => t.IsKeyword("AND", "OR")).Transform(t => new SqlOperatorNode(t)),
                        invertedBoolean,
                        (l, op, r) => new SqlInfixOperationNode
                        {
                            Location = op.Location,
                            Left = l,
                            Operator = op,
                            Right = r
                        }
                    )
            );

            booleanExpressionInternal = combinedBoolean;

            var orderDirection = First(
                Keyword("ASC"),
                Keyword("DESC")
            ).Optional();

            var orderColumn = Rule(
                First<SqlToken, ISqlNode>(
                    qualifiedIdentifier,
                    number
                ),
                orderDirection,
                (col, dir) => new SqlOrderByEntryNode
                {
                    Source = col,
                    Direction = dir?.Keyword ?? "ASC",
                    Location = col.Location
                }
            );
            var orderColumnList = orderColumn.ListSeparatedBy(comma, true);

            var selectOrderByClause = Rule(
                Keyword("ORDER", "BY"),
                orderColumnList,
                (k, cols) => new SqlOrderByNode
                {
                    Location = k.Location,
                    Entries = new SqlListNode<SqlOrderByEntryNode>(cols.ToList())
                }
            ).Optional();

            var rowOrRows = First(
                Keyword("ROW"),
                Keyword("ROWS")
            );

            var selectFetchClause = Rule(
                Keyword("FETCH"),
                First(
                    Keyword("FIRST"),
                    Keyword("NEXT")
                ),
                number,
                rowOrRows,
                Keyword("ONLY"),
                (f, n, qty, r, o) => qty
            ).Optional();

            var selectOffsetClause = Rule(
                Keyword("OFFSET"),
                number,
                rowOrRows,
                (o, qty, r) => qty
            ).Optional();

            

            

            var objectIdentifier = identifier
                .ListSeparatedBy(dot, 1, 4)
                .Transform(i => new SqlObjectIdentifierNode(i.ToList()));

            var valuesClause = Rule(
                Keyword("VALUES"),
                variableOrConstant
                    .ListSeparatedBy(comma, true)
                    .Transform(l => new SqlListNode<ISqlNode>(l.ToList()))
                    .Parenthesized()
                    .Transform(p => p.Expression)
                    .ListSeparatedBy(comma, true),
                (v, list) => new SqlValuesNode
                {
                    Location = v.Location,
                    Values = new SqlListNode<SqlListNode<ISqlNode>>(list.ToList())
                }
            );

            IParser<SqlToken, SqlSelectNode> selectQueryInternal = null;
            var selectQuery = Deferred(() => selectQueryInternal);

            var subexpression = First<SqlToken, ISqlNode>(
                selectQuery,
                valuesClause
            ).Parenthesized();

            var tableOrSubexpression = First<SqlToken, ISqlNode>(
                objectIdentifier,
                variable,
                subexpression
            );

            var joinOperatorRequiringOnClause = First(
                Keyword("CROSS", "APPLY").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Keyword("CROSS", "JOIN").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Keyword("INNER", "JOIN").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Keyword("OUTER", "APPLY").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Rule(
                    First(
                        Token(SqlTokenType.Keyword, "FULL"),
                        Token(SqlTokenType.Keyword, "LEFT"),
                        Token(SqlTokenType.Keyword, "RIGHT")
                    ).Optional(),
                    Token(SqlTokenType.Keyword, "OUTER").Optional(),
                    Token(SqlTokenType.Keyword, "JOIN"),
                    (scope, side, op) => new SqlOperatorNode(string.Join(" ", new[] { scope?.Value, side?.Value, op.Value }.Where(x => x != null)))
                )
            );
            var joinOperatorNotRequiringOnClause = First(
                Keyword("NATURAL", "JOIN").Transform(k => new SqlOperatorNode(k.Keyword, k.Location))
            );

            

            var joinCondition = Rule(
                Keyword("ON"),
                booleanExpression,
                (on, expr) => expr
            );

            var join = LeftApply(
                tableOrSubexpression,
                left => First(
                    Rule(
                        left,
                        joinOperatorRequiringOnClause,
                        tableOrSubexpression,
                        joinCondition,
                        (l, op, r, cond) => new SqlJoinNode
                        {
                            Location = l.Location,
                            Left = l,
                            Operator = op,
                            Right = r,
                            OnCondition = cond
                        }
                    ),
                    Rule(
                        left,
                        joinOperatorNotRequiringOnClause,
                        tableOrSubexpression,
                        (l, op, r) => new SqlJoinNode
                        {
                            Location = l.Location,
                            Left = l,
                            Operator = op,
                            Right = r
                        }
                    )
                )
            );

            var selectFromClause = Rule(
                Keyword("FROM"),
                join,
                (f, j) => j
            );

            var overPartition = Rule(
                Keyword("PARTITION", "BY"),
                scalarExpression.List(true).Transform(l => new SqlListNode<ISqlNode>(l.ToList())),
                (k, l) => l
            );
            var overOrderBy = Rule(
                Keyword("ORDER", "BY"),
                orderColumnList.Transform(l => new SqlListNode<SqlOrderByEntryNode>(l.ToList())),
                (k, cols) => cols
            );
            // TODO: This
            var overRows = Empty<SqlToken>().Transform(x => (ISqlNode)null);


            var selectColumnOverExpression = Rule(
                Keyword("OVER"),
                openParen,
                overPartition.Optional(),
                overOrderBy.Optional(),
                overRows.Optional(),
                closeParen,
                (k, o, p, ob, r, c) => new SqlOverNode {
                    Location = k.Location,
                    // TODO: HAve to invert this, the column has an over, not the over has an expression
                    Expression = null,
                    PartitionBy = p,
                    OrderBy = ob,
                    RowsRange = r
                }
            );

            var selectColumnExpression = Rule(
                scalarExpression,
                selectColumnOverExpression.Optional(),
                // TODO: Need to account for OVER
                // TODO: Create an SqlSelectColumnNode
                (expr, over) => expr
            );

            var selectColumn = First(
                star,
                // TODO: <variable> ('=' <scalarExpression>)?

                // TODO: maybe aliased
                selectColumnExpression
            );

            var selectColumnList = selectColumn.ListSeparatedBy(comma, true).Transform(l => new SqlListNode<ISqlNode>(l.ToList()));

            var selectHavingClause = Rule(
                Keyword("HAVING"),
                booleanExpression,
                (k, expr) => expr
            );
            // TODO: TOP clause (is that in standard or only T-SQL?)

            var selectWhereClause = Rule(
                Keyword("WHERE"),
                booleanExpression,
                (k, expr) => expr
            );

            var selectGroupByClause = Rule(
                Keyword("GROUP", "BY"),
                qualifiedIdentifier.ListSeparatedBy(comma, true),
                (k, list) => new SqlListNode<ISqlNode>(list.ToList())
            );

            var selectModifier = First(
                Keyword("ALL"),
                Keyword("DISTINCT")
            ).Optional();

            // TODO: Need to reduce the size of this list to 9 or less
            var querySpecification = Rule(
                Keyword("SELECT"),
                //selectModifier,
                // TODO: TOP
                selectColumnList,
                selectFromClause,
                selectWhereClause,
                selectGroupByClause,
                selectHavingClause,
                selectOrderByClause,
                selectOffsetClause,
                selectFetchClause,
                (k, /*mod,*/ cols, from, where, gb, having, ob, offset, fetch) => new SqlSelectNode
                {
                    Location = k.Location,
                    // TODO: "ALL"|"DISTINCT"
                    // TODO: TOP
                    //Modifier = mod,
                    Columns = cols,
                    FromClause = from,
                    WhereClause = where,
                    GroupByClause = gb,
                    HavingClause = having,
                    OrderByClause = ob,
                    OffsetClause = offset,
                    FetchClause = fetch
                }
            );

            var unionOperator = First(
                Keyword("UNION", "ALL"),
                Keyword("UNION"),
                Keyword("EXCEPT"),
                Keyword("INTERSECT")
            ).Transform(k => new SqlOperatorNode(k.Keyword, k.Location));

            queryExpressionInternal = LeftApply<SqlToken, ISqlNode>(
                querySpecification,
                left => Rule(
                    left,
                    unionOperator,
                    querySpecification,
                    (l, op, r) => new SqlInfixOperationNode
                    {
                        Location = l.Location,
                        Left = l,
                        Operator = op,
                        Right = r
                    }
                )
            );

            var unterminatedStatement = First(
                queryExpression,
                // TODO: Other expression types
                Empty<SqlToken>().Transform(x => (ISqlNode)null)
            );

            var statement = Rule(
                unterminatedStatement,
                Token(SqlTokenType.Symbol, ";"),
                (stmt, semicolon) => stmt
            );

            var statementList = statement.List().Transform(l => new SqlStatementListNode(l.ToList()));

            return statementList;
        }
    }
}