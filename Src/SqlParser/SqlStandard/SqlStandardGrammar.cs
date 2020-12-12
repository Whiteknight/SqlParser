using System.Linq;
using ParserObjects;
using SqlParser.Ast;
using SqlParser.Parsing;
using SqlParser.Tokenizing;
using static ParserObjects.ParserMethods<SqlParser.Tokenizing.SqlToken>;
using static SqlParser.Parsing.ParserMethods;

namespace SqlParser.SqlStandard
{
    public class SqlStandardGrammar
    {
        public SqlStandardGrammar()
        {
            Parser = InitializeParser();
        }

        public IParser<SqlToken, ISqlNode> Parser { get; }
        public static IParser<SqlToken, SqlToken> OpenParen { get; } = Token(SqlTokenType.Symbol, "(");
        public static IParser<SqlToken, SqlToken> CloseParen { get; } = Token(SqlTokenType.Symbol, ")");
        public static IParser<SqlToken, SqlToken> RequiredCloseParen { get; } = RequiredToken(SqlTokenType.Symbol, ")");

        private IParser<SqlToken, ISqlNode> InitializeParser()
        {
            var identifierToken = Token(SqlTokenType.Identifier);
            var identifier = identifierToken
                .Transform(t => new SqlIdentifierNode(t));
            var requiredIdentifier = First(
                identifier,
                ErrorNode<SqlIdentifierNode>("Expecting identifier")
            );

            var keywordToken = Token(SqlTokenType.Keyword);
            var keyword = keywordToken
                .Transform(k => new SqlKeywordNode(k.Value.ToUpperInvariant(), k.Location));

            var identifierOrKeywordAsIdentifier = First(
                identifierToken,
                Token(SqlTokenType.Keyword, "TARGET"),
                Token(SqlTokenType.Keyword, "SOURCE")
            ).Transform(t => new SqlIdentifierNode(t));

            var dot = Token(SqlTokenType.Symbol, ".");
            var star = Operator("*");
            var comma = Token(SqlTokenType.Symbol, ",");

            var equals = Operator("=");
            var assignmentOperator = Match(t => t.IsSymbol("=", "+=", "-=", "*=", "/=", "%=", "&=", "^=", "|="))
                .Transform(t => new SqlOperatorNode(t));

            var number = Token(SqlTokenType.Number)
                .Transform(t => new SqlNumberNode(t))
                .Named("number");
            var quotedString = Token(SqlTokenType.QuotedString).Transform(t => new SqlStringNode(t));
            var constant = First<ISqlNode>(
                quotedString,
                number
            );

            var variable = Token(SqlTokenType.Variable)
                .Transform(t => new SqlVariableNode(t))
                .Named("variable");

            var variableOrConstant = First(
                variable,
                constant
            );

            var queryExpressionInternal = Empty().Transform(x => (ISqlNode)null);
            var queryExpression = Deferred(() => queryExpressionInternal);

            var booleanExpressionInternal = Empty().Transform(x => (ISqlNode)null);
            var booleanExpression = Deferred(() => booleanExpressionInternal);

            var scalarExpressionInternal = Empty().Transform(x => (ISqlNode)null);
            var scalarExpression = Deferred(() => scalarExpressionInternal);

            var qualifierDotIdentifier = Rule(
                identifierOrKeywordAsIdentifier,
                dot,
                First<ISqlNode>(
                    star,
                    identifierOrKeywordAsIdentifier,
                    ErrorNode<SqlIdentifierNode>("Expecing '*' or Identifier")
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
            var variableOrQualifiedIdentifier = First(
                variable,
                qualifiedIdentifier
            );

            var countStarFunctionCall = Rule(
                Keyword("COUNT"),
                OpenParen,
                star,
                RequiredCloseParen,
                (c, o, s, x) => new SqlFunctionCallNode
                {
                    Location = c.Location,
                    Name = c,
                    Arguments = new SqlListNode<ISqlNode> { s }
                }
            );

            var dataTypeSize = First<ISqlNode>(
                    Keyword("MAX"),
                    number
                        .ListSeparatedBy(comma, true)
                        .Transform(n => new SqlListNode<SqlNumberNode>(n.ToList()))
                )
                .Parenthesized()
                .Transform(p => p.Expression);

            var dataType = Rule(
                keyword,
                // TODO: If we see an open paren we must require a datatype
                dataTypeSize.Optional(),
                (n, s) => new SqlDataTypeNode
                {
                    Location = n.Location,
                    DataType = n,
                    Size = s.GetValueOrDefault(null)
                }
            );

            var castFunctionCall = Rule(
                Keyword("CAST"),
                OpenParen,
                scalarExpression,
                Keyword("AS"),
                First(
                    dataType,
                    ErrorNode<SqlDataTypeNode>("Expecting data type")
                ),
                RequiredCloseParen,
                (k, o, expr, a, type, c) => new SqlCastNode
                {
                    Location = k.Location,
                    Expression = expr,
                    DataType = type
                }
            );

            var functionCall = First<ISqlNode>(
                countStarFunctionCall,
                castFunctionCall,
                Rule(
                    First<ISqlNode>(
                        keyword,
                        identifier
                    ),
                    // TODO: If we see an open paren, we must require a matching closed paren
                    scalarExpression
                        .ListSeparatedBy(comma)
                        .Transform(args => new SqlListNode<ISqlNode>(args.ToList()))
                        .Parenthesized(),
                    (name, args) => new SqlFunctionCallNode
                    {
                        Location = name.Location,
                        Name = name,
                        Arguments = args?.Expression
                    }
                )
            );

            var nullTerm = Match(t => t.IsKeyword("NULL")).Transform(t => new SqlNullNode(t));

            var scalarTerm = First(
                variable,
                quotedString,
                number,
                functionCall,
                qualifiedIdentifier,
                queryExpression.Parenthesized(),
                scalarExpression.Parenthesized().Transform(p => p.Expression)
            );

            // TODO: Make this rule properly recursive (right-associative) (NULL doesn't recurse and can't have the operators applied)
            // TODO: In SQL Standard the "||" operator is used to concat strings, but in SQL Server it is "+"
            var arithmeticPrefixOperator = Match(t => t.IsSymbol("-", "+", "~")).Transform(t => new SqlOperatorNode(t));
            var arithmeticPrefixInternal = Empty().Transform(t => (ISqlNode)null);
            var arithmeticPrefix = Deferred(() => arithmeticPrefixInternal);
            arithmeticPrefixInternal = First(
                nullTerm,
                scalarTerm,
                Rule(
                    // TODO: prefix operator not followed by NULL
                    arithmeticPrefixOperator,
                    arithmeticPrefix,
                    (op, expr) => new SqlPrefixOperationNode
                    {
                        Location = op.Location,
                        Operator = op,
                        Right = expr
                    }
                )
            );

            var multiplicative = LeftApply(
                arithmeticPrefix,
                left => Rule(
                    left,
                    Operator("*", "/", "%"),
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
                    Operator("+", "-", "&", "^", "|"),
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
                RequiredKeyword("THEN"),
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

            var requiredEnd = RequiredKeyword("END");

            var caseBlock = Rule(
                Keyword("CASE"),
                scalarExpression.MaybeParenthesized().Optional(),
                caseWhen.List(),
                caseElse.Optional(),
                requiredEnd,
                (c, expr, when, e, x) => new SqlCaseNode
                {
                    Location = c.Location,
                    InputExpression = expr.GetValueOrDefault(null),
                    WhenExpressions = when.ToList(),
                    ElseExpression = e.GetValueOrDefault(null)
                }
            );

            var caseExpression = First(
                caseBlock,
                additive
            );

            scalarExpressionInternal = caseExpression;

            // TODO: The "!=" operator is only for SQL Server, not SQL Standard. SQL Server
            // supports both "!=" and "<>". Standard only supports the later.
            var booleanComparison = Operator(">", "<", "=", "<=", ">=", "!=", "<>");
            var booleanComparisonModifier = First(
                Keyword("ALL"),
                Keyword("ANY"),
                Keyword("SOME")
            );

            var booleanTerm = LeftApply(
                scalarExpression,
                left => First<ISqlNode>(
                    Rule(
                        left,
                        booleanComparison,
                        booleanComparisonModifier.Optional(),
                        queryExpression.Parenthesized(),
                        (l, comp, mod, query) => new SqlInfixOperationNode
                        {
                            Location = l.Location,
                            Left = l,
                            Right = query,
                            Operator = mod.Success ? new SqlOperatorNode($"{comp.Operator} {mod.GetValueOrDefault(null)?.Keyword}", comp.Location) : comp
                        }
                    ),
                    Rule(
                        left,
                        booleanComparison,
                        scalarExpression,
                        (l, comp, r) => new SqlInfixOperationNode
                        {
                            Location = l.Location,
                            Left = l,
                            Right = r,
                            Operator = comp
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
                                Operator = !not.Success ? "IS" : "IS NOT"
                            },
                            Right = n
                        }
                    ),
                    Rule(
                        left,
                        Keyword("NOT").Optional(),
                        Keyword("BETWEEN"),
                        scalarExpression,
                        RequiredKeyword("AND"),
                        scalarExpression,
                        (l, not, between, expr1, and, expr2) => new SqlBetweenOperationNode
                        {
                            Location = between.Location,
                            Not = not.Success,
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
                            Not = not.Success,
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
                        (l, not, like, pattern) => new SqlInfixOperationNode
                        {
                            Left = l,
                            Operator = new SqlOperatorNode
                            {
                                Location = like.Location,
                                Operator = (not.Success ? "NOT " : "") + "LIKE",
                            },
                            Right = pattern
                        }
                    )
                ),
                Quantifier.ZeroOrOne
            );

            var existsExpression = Rule(
                Keyword("EXISTS"),
                First(
                    queryExpression.Parenthesized(),
                    ErrorNode<SqlParenthesisNode<ISqlNode>>("Expecting query")
                ),
                (kw, query) => new SqlPrefixOperationNode
                {
                    Location = kw.Location,
                    Operator = new SqlOperatorNode(kw.Keyword, kw.Location),
                    Right = query
                }
            );

            var booleanExists = First(
                existsExpression,
                // If we see a '(' at this point, it might be "(5) = 5" or it might be "(5 == 5)", so we
                // go to <booleanTerm> first because the scalar parser will cover the first case. Otherwise
                // we fallback to Parenthesized(<booleanExpression>) to try it that way. Extra backtracking
                // but it's what we have to do.
                booleanTerm,
                Parenthesized(
                    booleanExpression
                ).Transform(p => p.Expression)
            );

            var invertedBoolean = First(
                Rule(
                    Keyword("NOT"),
                    booleanExists,
                    (not, expr) => not == null
                        ? expr
                        : new SqlPrefixOperationNode
                        {
                            Operator = new SqlOperatorNode(not.Keyword, not.Location),
                            Location = not.Location,
                            Right = expr
                        }
                ),
                booleanExists
            );

            var combinedBoolean = LeftApply(
                invertedBoolean,
                left =>
                    Rule(
                        left,
                        Match(t => t.IsKeyword("AND", "OR")).Transform(t => new SqlOperatorNode(t)),
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
            // TODO: Should we insert a synthetic ASC here since it's the default?
            ).Optional();

            var orderColumn = Rule(
                First(
                    qualifiedIdentifier,
                    number
                ),
                orderDirection,
                (col, dir) => new SqlOrderByEntryNode
                {
                    Source = col,
                    // TODO: This should be an SqlKeywordNode
                    Direction = dir.GetValueOrDefault(null)?.Keyword,
                    Location = col.Location
                }
            );
            var orderColumnList = orderColumn.ListSeparatedBy(comma, true);

            var rowOrRows = First(
                Keyword("ROW"),
                Keyword("ROWS"),
                ErrorNode<SqlKeywordNode>("Expected 'ROW' or 'ROWS'")
            );

            var selectFetchClause = Rule(
                Keyword("FETCH"),
                First(
                    Keyword("FIRST"),
                    Keyword("NEXT")
                ),
                number,
                rowOrRows,
                RequiredKeyword("ONLY"),
                (f, n, qty, r, o) => qty
            );

            var selectOffsetClause = Rule(
                Keyword("OFFSET"),
                number,
                rowOrRows,
                (o, qty, r) => qty
            );

            var selectOrderByClause = Rule(
                Keyword("ORDER"),
                RequiredKeyword("BY"),
                orderColumnList,
                (order, by, cols) => new SqlOrderByNode
                {
                    // TODO: Integrate offset/fetch into this
                    Location = order.Location,
                    Entries = new SqlListNode<SqlOrderByEntryNode>(cols.ToList())
                }
            );

            var objectIdentifier = identifier
                .ListSeparatedBy(dot, 1, 4)
                .Transform(i => new SqlObjectIdentifierNode(i.ToList()));
            var variableOrObjectIdentifier = First<ISqlNode>(
                variable,
                objectIdentifier
            );
            var maybeAliasedTable = LeftApply<ISqlNode>(
                objectIdentifier,
                left => Rule(
                    left,
                    Keyword("AS"),
                    identifierOrKeywordAsIdentifier,
                    Parenthesized(
                        identifier.ListSeparatedBy(comma, true).Transform(l => new SqlListNode<SqlIdentifierNode>(l.ToList()))
                    ).Optional(),
                    (source, a, alias, columns) => new SqlAliasNode
                    {
                        Location = source.Location,
                        Source = source,
                        Alias = alias,
                        ColumnNames = columns.GetValueOrDefault(null)?.Expression
                    }
                ),
                Quantifier.ZeroOrOne
            );

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

            var subexpression = First(
                queryExpression,
                valuesClause
            ).Parenthesized();

            var tableOrSubexpression = First<ISqlNode>(
                objectIdentifier,
                variable,
                subexpression
            );

            var tableOrSubexpressionWithOptionalAlias = LeftApply(
                tableOrSubexpression,
                left => Rule(
                    left,
                    Keyword("AS").Optional(),
                    identifier,
                    // TODO: If we see an open paren, we must find a column list and a closed paren
                    Parenthesized(
                        identifier
                            .ListSeparatedBy(comma, true)
                            .Transform(l => new SqlListNode<SqlIdentifierNode>(l.ToList()))
                    ).Optional(),
                    (source, a, alias, columns) => new SqlAliasNode
                    {
                        Location = source.Location,
                        Source = source,
                        Alias = alias,
                        ColumnNames = columns.GetValueOrDefault(null)?.Expression
                    }
                ),
                Quantifier.ZeroOrOne
            );
            var requiredTableOrSubexpressionWithOptionalAlias = First(
                tableOrSubexpressionWithOptionalAlias,
                ErrorNode<SqlIdentifierNode>("Expecting table or subexpression")
            );

            var joinOperatorRequiringOnClause = First(
                Keyword("CROSS", "APPLY").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Keyword("CROSS", "JOIN").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Keyword("INNER", "JOIN").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                Keyword("OUTER", "APPLY").Transform(k => new SqlOperatorNode(k.Keyword, k.Location)),
                // TODO: hints: "MERGE" | "HASH" | "REDISTRIBUTE" | "REPLICATE" | "REDUCE"
                Rule(
                    First(
                        Token(SqlTokenType.Keyword, "FULL"),
                        Token(SqlTokenType.Keyword, "LEFT"),
                        Token(SqlTokenType.Keyword, "RIGHT")
                    ).Optional(),
                    Token(SqlTokenType.Keyword, "OUTER").Optional(),
                    Token(SqlTokenType.Keyword, "JOIN"),
                    (scope, side, op) => new SqlOperatorNode(string.Join(" ", new[] { scope.GetValueOrDefault(null)?.Value, side.GetValueOrDefault(null)?.Value, op.Value }.Where(x => x != null)))
                )
            );
            var joinOperatorNotRequiringOnClause = First(
                Keyword("NATURAL", "JOIN").Transform(k => new SqlOperatorNode(k.Keyword, k.Location))
            );

            var joinCondition = First(
                Rule(
                    Keyword("ON"),
                    booleanExpression,
                    (on, expr) => expr
                ),
                ErrorNode<SqlIdentifierNode>("Required ON <condition>")
            );

            var join = LeftApply(
                tableOrSubexpressionWithOptionalAlias,
                // TODO: <TableExpression> ("WITH" <Hint>)?
                left => First(
                    Rule(
                        left,
                        joinOperatorRequiringOnClause,
                        requiredTableOrSubexpressionWithOptionalAlias,
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
                        requiredTableOrSubexpressionWithOptionalAlias,
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
                Keyword("PARTITION"),
                RequiredKeyword("BY"),
                scalarExpression.List(true).Transform(l => new SqlListNode<ISqlNode>(l.ToList())),
                (partition, by, l) => l
            );

            var overOrderBy = Rule(
                Keyword("ORDER"),
                RequiredKeyword("BY"),
                orderColumnList.Transform(l => new SqlListNode<SqlOrderByEntryNode>(l.ToList())),
                (order, by, cols) => cols
            );

            // TODO: This
            var overRows = Empty().Transform(x => (ISqlNode)null);

            var variableColumn = First<ISqlNode>(
                Rule(
                    variable,
                    Match(t => t.IsSymbol("=")).Transform(t => new SqlOperatorNode(t)),
                    scalarExpression,
                    (var, op, expr) => new SqlInfixOperationNode
                    {
                        Location = var.Location,
                        Left = var,
                        Operator = op,
                        Right = expr
                    }
                ),
                variable
            );

            // TODO: Create an SqlSelectColumnNode
            var selectColumnExpression = LeftApply(
                scalarExpression,
                left => Rule(
                    left,
                    Keyword("OVER"),
                    OpenParen,
                    // TODO: We need at least one clause here
                    overPartition.Optional(),
                    overOrderBy.Optional(),
                    overRows.Optional(),
                    CloseParen,
                    (l, k, o, p, ob, r, c) => new SqlOverNode
                    {
                        Location = k.Location,
                        // TODO: Have to invert this, the column has an over, not the over has an expression
                        Expression = l,
                        PartitionBy = p.GetValueOrDefault(null),
                        OrderBy = ob.GetValueOrDefault(null),
                        RowsRange = r.GetValueOrDefault(null)
                    }
                )
            );

            var aliasableColumn = First(
                variableColumn,
                selectColumnExpression
            );

            var maybeAliasedColumn = LeftApply(
                aliasableColumn,
                left => Rule(
                    left,
                    Keyword("AS"),
                    requiredIdentifier,
                    (col, asOp, name) => new SqlAliasNode
                    {
                        Location = col.Location,
                        Source = col,
                        Alias = name
                    }
                )
            );

            var selectColumn = First(
                star,
                maybeAliasedColumn
            );

            var selectColumnList = selectColumn
                .ListSeparatedBy(comma, true)
                .Transform(l => new SqlListNode<ISqlNode>(l.ToList()));

            var selectHavingClause = Rule(
                Keyword("HAVING"),
                booleanExpression,
                (k, expr) => expr
            );

            var whereClause = Rule(
                Keyword("WHERE"),
                First(
                    booleanExpression,
                    ErrorNode<SqlIdentifierNode>("Expecting condition")
                ),
                (k, expr) => expr
            );

            var selectGroupByClause = Rule(
                Keyword("GROUP"),
                RequiredKeyword("BY"),
                // TODO: At this point we must require at least one identifier
                qualifiedIdentifier.ListSeparatedBy(comma, true),
                (group, by, list) => new SqlListNode<ISqlNode>(list.ToList())
            );

            var selectModifier = First(
                Keyword("ALL"),
                Keyword("DISTINCT")
            );

            var keywordSelect = Keyword("SELECT");

            var selectClause = Rule(
                keywordSelect,
                selectModifier.Optional(),
                Empty().Transform(_ => (SqlTopLimitNode)null).Replaceable("selectTopClause"),
                selectColumnList,
                // TODO: This should return a Select Clause Node
                (select, mod, top, columns) => new
                {
                    Select = select,
                    Modifiers = mod,
                    Top = top,
                    Columns = columns
                }
            );

            // TODO: Need to reduce the size of this list to 9 or less
            var querySpecification = Rule(
                selectClause,
                // TODO: I think FROM clause is mandatory in SQL standard, optional only in SQL Server
                selectFromClause.Optional(),
                whereClause.Optional(),
                selectGroupByClause.Optional(),
                selectHavingClause.Optional(),
                selectOrderByClause.Optional(),
                // TODO: In SQL Standard I think ORDER BY is optional with OFFSET/FETCH, but in
                // SQL Server it might be mandatory.
                selectOffsetClause.Optional(),
                selectFetchClause.Optional(),
                // TODO: This should return a QuerySpecificationNode which takes an SelectClauseNode
                (select, from, where, gb, having, orderBy, offset, fetch) => new SqlSelectNode
                {
                    Location = select.Select.Location,
                    TopLimitClause = select.Top,
                    // TODO: .Modifier should be an SqlKeywordNode
                    Modifier = select.Modifiers.GetValueOrDefault(null)?.Keyword,
                    Columns = select.Columns,
                    FromClause = from.GetValueOrDefault(null),
                    WhereClause = where.GetValueOrDefault(null),
                    GroupByClause = gb.GetValueOrDefault(null),
                    HavingClause = having.GetValueOrDefault(null),
                    OrderByClause = orderBy.GetValueOrDefault(null),
                    OffsetClause = offset.GetValueOrDefault(null),
                    FetchClause = fetch.GetValueOrDefault(null)
                }
            );

            var unionOperator = First(
                    Keyword("UNION", "ALL"),
                    Keyword("UNION"),
                    Keyword("EXCEPT"),
                    Keyword("INTERSECT")
                )
                .Transform(k => new SqlOperatorNode(k.Keyword, k.Location));

            queryExpressionInternal = RightApply<SqlOperatorNode, ISqlNode>(
                querySpecification,
                unionOperator,
                (l, op, r) => new SqlInfixOperationNode
                {
                    Location = l.Location,
                    Left = l,
                    Operator = op,
                    Right = r
                }
            );

            var deleteStatement = Rule(
                Keyword("DELETE"),
                RequiredKeyword("FROM"),
                // TODO: TOP clause
                First(
                    objectIdentifier,
                    ErrorNode<SqlObjectIdentifierNode>("Expecting table name")
                ),
                whereClause.Optional(),
                // TODO: SQL Server supports an extra FROM clause to select records to delete based
                // on values from another query
                // TODO: SQL Server supports "DELETE A FROM A JOIN B" syntax
                // TODO: OUTPUT clause
                (delete, from, id, where) => new SqlDeleteNode
                {
                    Location = delete.Location,
                    Source = id,
                    WhereClause = where.GetValueOrDefault(null)
                }
            );

            var outputKeyword = First(
                Keyword("OUTPUT"),
                Keyword("OUT")
            );
            var maybeOutputKeyword = outputKeyword.Optional();
            var requiredScalarExpression = First(
                scalarExpression,
                ErrorNode<SqlIdentifierNode>("Expecting expression")
            );

            var execArgument = First(
                Keyword("DEFAULT").Transform(k =>
                    new SqlExecuteArgumentNode
                    {
                        Location = k.Location,
                        Value = k
                    }
                ),
                Rule(
                    variable,
                    equals,
                    requiredScalarExpression,
                    maybeOutputKeyword,
                    (v, eq, expr, output) => new SqlExecuteArgumentNode
                    {
                        Location = v.Location,
                        AssignVariable = v,
                        Value = expr,
                        IsOut = output.Success
                    }
                ),
                Rule(
                    scalarExpression,
                    maybeOutputKeyword,
                    (expr, output) => new SqlExecuteArgumentNode
                    {
                        Location = expr.Location,
                        Value = expr,
                        IsOut = output.Success
                    }
                )
            );

            var executeStatement = LeftApply<ISqlNode>(
                // TODO: @return_status = ...
                First(
                    Keyword("EXECUTE"),
                    Keyword("EXEC")
                ),
                left => First<ISqlNode>(
                    Rule(
                        left,
                        // TODO: WITH <execute_option>
                        objectIdentifier,
                        execArgument.ListSeparatedBy(comma).Transform(l => new SqlListNode<SqlExecuteArgumentNode>(l.ToList())),
                        (exec, name, args) => new SqlExecuteNode
                        {
                            Location = exec.Location,
                            Name = name,
                            Arguments = args
                        }
                    ),
                    Rule(
                        left,
                        scalarExpression.Parenthesized(),
                        (exec, expr) => new SqlExecuteNode
                        {
                            Location = exec.Location,
                            Name = expr
                        }
                    ),
                    Rule(
                        left,
                        quotedString,
                        (exec, str) => new SqlExecuteNode
                        {
                            Location = exec.Location,
                            Name = str
                        }
                    ),
                    Rule(
                        left,
                        variable,
                        (exec, str) => new SqlExecuteNode
                        {
                            Location = exec.Location,
                            Name = str
                        }
                    )
                ),
                Quantifier.ExactlyOne
            );

            var insertColumnList = identifier
                .ListSeparatedBy(comma, true)
                .Transform(l => new SqlListNode<SqlIdentifierNode>(l.ToList()))
                .Parenthesized();

            var valuesLiteral = Rule(
                Keyword("VALUES"),
                // TODO: We must require the open and closed parens, and at least one value
                variableOrConstant
                    .ListSeparatedBy(comma, true)
                    .Transform(t => new SqlListNode<ISqlNode>(t.ToList()))
                    .Parenthesized()
                    .Transform(p => p.Expression)
                    .ListSeparatedBy(comma, true)
                    .Transform(t => new SqlListNode<SqlListNode<ISqlNode>>(t.ToList())),
                (values, list) => new SqlValuesNode
                {
                    Location = values.Location,
                    Values = list
                }
            );

            var insertSource = First(
                valuesLiteral,
                // TODO: If we see DEFAULT we must see VALUES
                Keyword("DEFAULT", "VALUES"),
                // TODO: "INSERT INTO ... SELECT ..." from is only supported by SQL Server, not standard
                queryExpression,
                executeStatement
            // TODO: EXEC/EXECUTE
            // TODO: DEFAULT VALUES
            );

            var insertStatement = Rule(
                Keyword("INSERT"),
                RequiredKeyword("INTO"),
                objectIdentifier,
                insertColumnList,
                insertSource,
                (insert, into, table, columns, source) => new SqlInsertNode
                {
                    Location = insert.Location,
                    Table = table,
                    Columns = columns.Expression,
                    Source = source
                }
            );

            var updateColumnAssignExpression = Rule(
                variableOrQualifiedIdentifier,
                assignmentOperator,
                First(
                    Keyword("DEFAULT"),
                    scalarExpression,
                    ErrorNode<SqlIdentifierNode>("Expecting 'DEFAULT' or expression")
                ),
                (name, op, expr) => new SqlInfixOperationNode
                {
                    Left = name,
                    Location = name.Location,
                    Operator = op,
                    Right = expr
                }
            );

            var updateSetClause = Rule(
                Keyword("SET"),
                // TODO: Require at least one set expression
                updateColumnAssignExpression.ListSeparatedBy(comma, true).Transform(l => new SqlListNode<SqlInfixOperationNode>(l.ToList())),
                (set, assigns) => assigns
            );

            var updateStatement = Rule(
                Keyword("UPDATE"),
                // TODO: TOP clause
                // TODO: Alias
                First(
                    variableOrObjectIdentifier,
                    ErrorNode<SqlVariableNode>("Expecting table variable or table name")
                ),
                First(
                    updateSetClause,
                    ErrorNode<SqlListNode<SqlInfixOperationNode>>("Expected SET clause")
                ),
                // TODO: SQL Server provides a "FROM" syntax to delete from table B based on values from a query on table A
                // TODO: OUTPUT clause, which is only supported by SQL Server
                whereClause.Optional(),
                (update, table, set, where) => new SqlUpdateNode
                {
                    Location = update.Location,
                    Source = table,
                    SetClause = set,
                    WhereClause = where.GetValueOrDefault(null)
                }
            );

            var mergeOnMatched = First<ISqlNode>(
                Rule(
                    Keyword("UPDATE"),
                    updateSetClause,
                    (update, set) => new SqlUpdateNode
                    {
                        Location = update.Location,
                        SetClause = set
                    }
                ),
                Keyword("DELETE"),
                ErrorNode<SqlKeywordNode>("Expected DELETE or UPDATE")
            );
            var mergeOnNotMatched = First<ISqlNode>(
                Rule(
                    Keyword("INSERT"),
                    insertColumnList,
                    valuesLiteral,
                    (insert, columns, values) => new SqlInsertNode
                    {
                        Location = insert.Location,
                        Columns = columns.Expression,
                        Source = values
                    }
                ),
                ErrorNode<SqlKeywordNode>("Expected INSERT")
            );

            var mergeMatchedClause = Rule(
                Keyword("WHEN"),
                Keyword("MATCHED"),
                // TODO: "AND" <clauseSearchCondition>
                Keyword("THEN"),
                mergeOnMatched,
                (when, matched, then, onMatched) => new SqlMergeMatchClauseNode
                {
                    Location = when.Location,
                    Keyword = new SqlKeywordNode("WHEN MATCHED", when.Location),
                    Action = onMatched
                }
            );
            var mergeNotMatchedByTarget = First(
                Rule(
                    Keyword("WHEN"),
                    Keyword("NOT"),
                    Keyword("MATCHED"),
                    Keyword("BY"),
                    Keyword("TARGET"),
                    // TODO: "AND" <clauseSearchCondition>
                    Keyword("THEN"),
                    mergeOnNotMatched,
                    (when, not, matched, by, target, then, onNotMatched) => new SqlMergeMatchClauseNode
                    {
                        Location = when.Location,
                        Keyword = new SqlKeywordNode("WHEN NOT MATCHED BY TARGET", when.Location),
                        Action = onNotMatched
                    }
                ),
                Rule(
                    Keyword("WHEN"),
                    Keyword("NOT"),
                    Keyword("MATCHED"),
                    // TODO: "AND" <clauseSearchCondition>
                    Keyword("THEN"),
                    mergeOnNotMatched,
                    (when, not, matched, then, onNotMatched) => new SqlMergeMatchClauseNode
                    {
                        Location = when.Location,
                        Keyword = new SqlKeywordNode("WHEN NOT MATCHED", when.Location),
                        Action = onNotMatched
                    }
                )
            );

            var mergeNotMatchedBySource = Rule(
                Keyword("WHEN"),
                Keyword("NOT"),
                Keyword("MATCHED"),
                Keyword("BY"),
                Keyword("SOURCE"),
                // TODO: "AND" <clauseSearchCondition>
                Keyword("THEN"),
                mergeOnMatched,
                (when, not, matched, by, source, then, onMatched) => new SqlMergeMatchClauseNode
                {
                    Location = when.Location,
                    Keyword = new SqlKeywordNode("WHEN NOT MATCHED BY SOURCE", when.Location),
                    Action = onMatched
                }
            );

            var mergeMatchingClause = First(
                mergeMatchedClause,
                mergeNotMatchedByTarget,
                mergeNotMatchedBySource
            );

            var mergeIntoClause = Rule(
                Keyword("INTO").Optional(),
                maybeAliasedTable,
                (into, table) => table
            );
            var mergeUsingClause = Rule(
                Keyword("USING"),
                maybeAliasedTable,
                (u, table) => table
            );
            var mergeOnClause = Rule(
                Keyword("ON"),
                booleanExpression,
                (on, cond) => cond
            );

            var mergeStatement = Rule(
                Keyword("MERGE"),
                mergeIntoClause,
                mergeUsingClause,
                mergeOnClause,
                mergeMatchingClause.List(true),
                // TODO: Output clause
                // TODO: OPTION clause
                (merge, into, u, on, matched) => new SqlMergeNode
                {
                    Location = merge.Location,
                    Target = into,
                    Source = u,
                    MergeCondition = on,
                    MatchClauses = new SqlListNode<SqlMergeMatchClauseNode>(matched.ToList())
                }
            );

            var withCte = Rule(
                identifier,
                identifier.ListSeparatedBy(comma, true).Transform(l => new SqlListNode<SqlIdentifierNode>(l.ToList())).Parenthesized().Optional(),
                RequiredKeyword("AS"),
                First(
                    queryExpression.Parenthesized().Transform(p => p.Expression),
                    ErrorNode<SqlSelectNode>("Expected query expression")
                ),
                (id, cols, a, query) =>
                {
                    var cte = new SqlWithCteNode
                    {
                        Location = id.Location,
                        Name = id,
                        ColumnNames = cols.GetValueOrDefault(null)?.Expression,
                        Select = query
                    };
                    cte.DetectRecursion();
                    return cte;
                }
            );

            var withChildStatement = First(
                queryExpression,
                deleteStatement,
                insertStatement,
                updateStatement,
                mergeStatement,
                ErrorNode<SqlIdentifierNode>("Expecting SELECT, DELETE, INSERT, UPDATE or MERGE")
            );

            var withStatement = Rule(
                Keyword("WITH"),
                withCte.ListSeparatedBy(comma, true).Transform(c => new SqlListNode<SqlWithCteNode>(c.ToList())),
                withChildStatement,
                (with, ctes, child) => new SqlWithNode
                {
                    Location = with.Location,
                    Ctes = ctes,
                    Statement = child
                }
            );

            var assignmentExpression = Rule(
                Match(t => t.IsSymbol("=")),
                scalarExpression,
                (eq, expr) => expr
            );
            var requiredVariable = First(
                variable,
                ErrorNode<SqlVariableNode>("Expecting variable")
            );

            var declareStatement = Rule(
                // TODO: "DECLARE" <variable> <DataType> ("=" <Expression>)? ("," <variable> <DataType> ("=" <Expression>)?)*
                Keyword("DECLARE"),
                requiredVariable,
                First(
                    dataType,
                    ErrorNode<SqlDataTypeNode>("Expecting data type")
                ),
                assignmentExpression.Optional(),
                (declare, vari, type, init) => new SqlDeclareNode
                {
                    Location = declare.Location,
                    Variable = vari,
                    DataType = type,
                    Initializer = init.GetValueOrDefault(null)
                }
            );

            var setStatement = Rule(
                Keyword("SET"),
                requiredVariable,
                First(
                    assignmentOperator,
                    ErrorNode<SqlOperatorNode>("Expecting assignment operator")
                ),
                requiredScalarExpression,
                (set, v, op, expr) => new SqlSetNode
                {
                    Location = set.Location,
                    Variable = v,
                    Operator = op,
                    Right = expr
                }
            );

            var statementInternal = Empty().Transform(t => (ISqlNode)null);
            var statement = Deferred(() => statementInternal);
            var statementList = statement.List().Transform(l =>
                new SqlStatementListNode(l.ToList()));

            var ifStatement = Rule(
                Keyword("IF"),
                booleanExpression.MaybeParenthesized(),
                statement,
                Rule(
                    Keyword("ELSE"),
                    statement,
                    (e, stmt) => stmt
                ).Optional(),
                (i, cond, onThen, onElse) => new SqlIfNode
                {
                    Location = i.Location,
                    Condition = cond,
                    Then = onThen,
                    Else = onElse.GetValueOrDefault(null)
                }
            );

            var beginBlock = Rule(
                Keyword("BEGIN"),
                statementList,
                Keyword("END"),
                (begin, stmts, e) =>
                {
                    stmts.UseBeginEnd = true;
                    return stmts;
                }
            );

            // TODO: "GO" which starts a new logical block and also sets scope limits for variables

            var unterminatedStatement = First(
                withStatement,
                queryExpression,
                declareStatement,
                setStatement,
                deleteStatement,
                insertStatement,
                updateStatement,
                mergeStatement,
                executeStatement,
                ifStatement,
                beginBlock
            // TODO: If we don't know what it is, Parse it into an Unknown node and continue
            // TODO: RETURN?
            // TODO: THROW/TRY/CATCH
            // TODO: WHILE/CONTINUE/BREAK
            // TODO: CREATE/DROP/ALTER? Do we want to handle DDL statments here?
            //Empty().Transform(x => (ISqlNode)null)
            );

            statementInternal = Rule(
                unterminatedStatement,
                Token(SqlTokenType.Symbol, ";").Optional(),
                (stmt, semicolon) => stmt
            );

            //return statementList;
            return Rule(
                statementList,
                First(
                    If(End(), Produce(() => (SqlToken)null)),
                    Match(t => t.Type == SqlTokenType.EndOfInput)
                ),
                (stmts, eof) => stmts
            );
        }
    }
}
