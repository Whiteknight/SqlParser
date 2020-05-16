using System.Linq;
using SqlParser.Ast;
using SqlParser.SqlServer.Validation;
using SqlParser.SqlStandard;
using SqlParser.Visiting;

namespace SqlParser.PostgreSql.Validation
{
    // TODO: Fill in all remaining validations
    // TODO: Fix abstraction boundary between this visitor and the ValidationResult
    public class SqlNodeValidateVisitor : SqlNodeVisitor
    {
        private readonly ValidationResult _result;

        public SqlNodeValidateVisitor(ValidationResult result)
        {
            _result = result;
        }

        public override ISqlNode VisitAlias(SqlAliasNode n)
        {
            _result.AssertNotNull(n, nameof(n.Source), n.Source);
            _result.AssertNotNull(n, nameof(n.Alias), n.Alias);
            return base.VisitAlias(n);
        }

        public override ISqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            _result.AssertIsScalarExpression(n, nameof(n.Left), n.Left);
            _result.AssertIsScalarExpression(n, nameof(n.Low), n.Low);
            _result.AssertIsScalarExpression(n, nameof(n.High), n.High);
            return base.VisitBetween(n);
        }

        public override ISqlNode VisitCase(SqlCaseNode n)
        {
            _result.AssertIsScalarExpression(n, nameof(n.InputExpression), n.InputExpression);
            if (n.ElseExpression != null)
                _result.AssertIsScalarExpression(n, nameof(n.ElseExpression), n.ElseExpression);
            return base.VisitCase(n);
        }

        public override ISqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            _result.AssertIsScalarExpression(n, nameof(n.Condition), n.Condition);
            _result.AssertIsScalarExpression(n, nameof(n.Result), n.Result);
            return base.VisitCaseWhen(n);
        }

        public override ISqlNode VisitDataType(SqlDataTypeNode n)
        {
            _result.AssertNotNull(n, nameof(n.DataType), n.DataType);
            if (n.Size == null) { }
            else if (n.Size is SqlKeywordNode isMax)
                _result.AssertIsValue(n, nameof(n.Size), isMax.Keyword, "MAX");
            else if (n.Size is SqlNumberNode asNumber)
                _result.AssertIsPositiveNumber(n, nameof(n.Size), asNumber.Numeric);
            else if (n.Size is SqlListNode<SqlNumberNode> asList)
            {
                _result.AssertIsNotEmpty(n, nameof(n.Size), asList);
                asList.Select((i, x) => _result.AssertIsPositiveNumber(asList, x.ToString(), i.Numeric)).All(x => x);
            }
            else
                _result.UnexpectedNodeType(n, nameof(n.Size), n.Size);
            return base.VisitDataType(n);
        }

        public override ISqlNode VisitDeclare(SqlDeclareNode n)
        {
            return base.VisitDeclare(n);
        }

        public override ISqlNode VisitDelete(SqlDeleteNode n)
        {
            return base.VisitDelete(n);
        }

        public override ISqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            return base.VisitFunctionCall(n);
        }

        public override ISqlNode VisitIdentifier(SqlIdentifierNode n)
        {
            return base.VisitIdentifier(n);
        }

        public override ISqlNode VisitIn(SqlInNode n)
        {
            _result.AssertIsNotEmpty(n, nameof(n.Items), n.Items);
            n.Items.Select((i, x) => _result.AssertIsScalarExpression(n.Items, x.ToString(), i)).All(x => x);
            return base.VisitIn(n);
        }

        public override ISqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            // TODO: Can we assert a more specific requirement on Left and Right?
            _result.AssertNotNull(n, nameof(n.Left), n.Left);
            _result.AssertNotNull(n, nameof(n.Operator), n.Operator);
            _result.AssertNotNull(n, nameof(n.Right), n.Right);
            if (n.IsUnionOperation())
            {
                _result.AssertIsUnionStatement(n, nameof(n.Left), n.Left);
                _result.AssertIsUnionStatement(n, nameof(n.Right), n.Right);
            }
            else if (n.IsArithmeticOperation())
            {
                _result.AssertIsScalarExpression(n, nameof(n.Left), n.Left);
                _result.AssertIsScalarExpression(n, nameof(n.Right), n.Right);
            }
            else if (n.IsBooleanOperation())
            {
                _result.AssertIsBooleanExpression(n, nameof(n.Left), n.Left);
                _result.AssertIsBooleanExpression(n, nameof(n.Right), n.Right);
            }
            else if (n.IsComparisonOperation())
            {
                _result.AssertIsScalarExpression(n, nameof(n.Left), n.Left);
                _result.AssertIsScalarExpression(n, nameof(n.Right), n.Right);
            }
            else
                _result.UnexpectedNodeType(n, nameof(n.Operator), n.Operator);
            return base.VisitInfixOperation(n);
        }

        public override ISqlNode VisitInsert(SqlInsertNode n)
        {
            _result.AssertNotNull(n, nameof(n.Columns), n.Columns);
            if (n.Columns.Count == 0)
                _result.AddError(n, nameof(n.Columns), "Must specify at least one column");
            _result.AssertNotNull(n, nameof(n.Source), n.Source);
            return base.VisitInsert(n);
        }

        public override ISqlNode VisitValues(SqlValuesNode n)
        {
            return base.VisitValues(n);
        }

        public override SqlJoinNode VisitJoin(SqlJoinNode n)
        {
            return base.VisitJoin(n);
        }

        public override ISqlNode VisitKeyword(SqlKeywordNode n)
        {
            _result.AssertIsNotNullOrEmpty(n, nameof(n.Keyword), n.Keyword);
            return base.VisitKeyword(n);
        }

        public override ISqlNode VisitList<TNode>(SqlListNode<TNode> n)
        {
            return base.VisitList(n);
        }

        public override ISqlNode VisitMerge(SqlMergeNode n)
        {
            _result.AssertNotNull(n, nameof(n.Target), n.Target);
            _result.AssertNotNull(n, nameof(n.Source), n.Source);
            _result.AssertNotNull(n, nameof(n.MergeCondition), n.MergeCondition);
            // TODO: This
            //if (n.Matched != null)
            //{
            //    if (!(n.Matched is SqlUpdateNode || (n.Matched is SqlKeywordNode keyword && keyword.Keyword == "DELETE")))
            //        _result.AddError(n, nameof(n.Matched), "MATCHED clause must be valid UPDATE or DELETE");
            //}
            //if (n.NotMatchedBySource != null)
            //{
            //    if (!(n.NotMatchedBySource is SqlUpdateNode || (n.NotMatchedBySource is SqlKeywordNode keyword && keyword.Keyword == "DELETE")))
            //        _result.AddError(n, nameof(n.NotMatchedBySource), "NOT MATCHED BY SOURCE clause must be valid UPDATE or DELETE");
            //}
            return base.VisitMerge(n);
        }

        public override ISqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
        {
            return base.VisitObjectIdentifier(n);
        }

        public override ISqlNode VisitOperator(SqlOperatorNode n)
        {
            _result.AssertIsNotNullOrEmpty(n, nameof(n.Operator), n.Operator);
            return base.VisitOperator(n);
        }

        public override ISqlNode VisitOrderBy(SqlOrderByNode n)
        {
            return base.VisitOrderBy(n);
        }

        public override ISqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            return base.VisitOrderByEntry(n);
        }

        public override ISqlNode VisitOver(SqlOverNode n)
        {
            return base.VisitOver(n);
        }

        public override ISqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
        {
            _result.AssertNotNull(n, nameof(n.Expression), n.Expression);
            return base.VisitParenthesis(n);
        }

        public override ISqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            _result.AssertNotNull(n, nameof(n.Operator), n.Operator);
            _result.AssertNotNull(n, nameof(n.Right), n.Right);
            return base.VisitPrefixOperation(n);
        }

        public override ISqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            return base.VisitQualifiedIdentifier(n);
        }

        public override ISqlNode VisitSelect(SqlSelectNode n)
        {
            _result.AssertIsNotNullOrEmpty(n, nameof(n.Columns), n.Columns);
            // TODO: If the FROM clause contains a SELECT or values subquery, it MUST be in a parenthesis node
            // TODO: If the FROM clause contains a VALUES expression, it MUST be aliased and have ColumnNames
            return base.VisitSelect(n);
        }

        public override ISqlNode VisitSet(SqlSetNode n)
        {
            return base.VisitSet(n);
        }

        public override ISqlNode VisitStatementList(SqlStatementListNode n)
        {
            return base.VisitStatementList(n);
        }

        public override ISqlNode VisitTopLimit(SqlTopLimitNode n)
        {
            return base.VisitTopLimit(n);
        }

        public override ISqlNode VisitWith(SqlWithNode n)
        {
            return base.VisitWith(n);
        }

        public override ISqlNode VisitWithCte(SqlWithCteNode n)
        {
            // TODO: Detect recursive CTE and validate correct structure
            return base.VisitWithCte(n);
        }

        public override ISqlNode VisitUpdate(SqlUpdateNode n)
        {
            return base.VisitUpdate(n);
        }

        public override ISqlNode VisitVariable(SqlVariableNode n)
        {
            return base.VisitVariable(n);
        }
    }
}
