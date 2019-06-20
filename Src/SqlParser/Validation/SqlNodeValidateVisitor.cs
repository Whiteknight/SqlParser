using System.Linq;
using SqlParser.Ast;

namespace SqlParser.Validation
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

        public override SqlNode VisitAlias(SqlAliasNode n)
        {
            _result.AssertNotNull(n, nameof(n.Source), n.Source);
            _result.AssertNotNull(n, nameof(n.Alias), n.Alias);
            return base.VisitAlias(n);
        }

        public override SqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            _result.AssertIsScalarExpression(n, nameof(n.Left), n.Left);
            _result.AssertIsScalarExpression(n, nameof(n.Low), n.Low);
            _result.AssertIsScalarExpression(n, nameof(n.High), n.High);
            return base.VisitBetween(n);
        }

        public override SqlNode VisitCase(SqlCaseNode n)
        {
            _result.AssertIsScalarExpression(n, nameof(n.InputExpression), n.InputExpression);
            if (n.ElseExpression != null)
                _result.AssertIsScalarExpression(n, nameof(n.ElseExpression), n.ElseExpression);
            return base.VisitCase(n);
        }

        public override SqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            _result.AssertIsScalarExpression(n, nameof(n.Condition), n.Condition);
            _result.AssertIsScalarExpression(n, nameof(n.Result), n.Result);
            return base.VisitCaseWhen(n);
        }

        public override SqlNode VisitDataType(SqlDataTypeNode n)
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

        public override SqlNode VisitDeclare(SqlDeclareNode n)
        {
            return base.VisitDeclare(n);
        }

        public override SqlNode VisitDelete(SqlDeleteNode n)
        {
            return base.VisitDelete(n);
        }

        public override SqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            return base.VisitFunctionCall(n);
        }

        public override SqlNode VisitIdentifier(SqlIdentifierNode n)
        {
            return base.VisitIdentifier(n);
        }

        public override SqlNode VisitIn(SqlInNode n)
        {
            _result.AssertIsNotEmpty(n, nameof(n.Items), n.Items);
            n.Items.Select((i, x) => _result.AssertIsScalarExpression(n.Items, x.ToString(), i)).All(x => x);
            return base.VisitIn(n);
        }

        public override SqlNode VisitInfixOperation(SqlInfixOperationNode n)
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

        public override SqlNode VisitInsert(SqlInsertNode n)
        {
            return base.VisitInsert(n);
        }

        public override SqlNode VisitValues(SqlValuesNode n)
        {
            return base.VisitValues(n);
        }

        public override SqlJoinNode VisitJoin(SqlJoinNode n)
        {
            return base.VisitJoin(n);
        }

        public override SqlNode VisitKeyword(SqlKeywordNode n)
        {
            _result.AssertIsNotNullOrEmpty(n, nameof(n.Keyword), n.Keyword);
            return base.VisitKeyword(n);
        }

        public override SqlNode VisitList<TNode>(SqlListNode<TNode> n)
        {
            return base.VisitList(n);
        }

        public override SqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
        {
            return base.VisitObjectIdentifier(n);
        }

        public override SqlNode VisitOperator(SqlOperatorNode n)
        {
            _result.AssertIsNotNullOrEmpty(n, nameof(n.Operator), n.Operator);
            return base.VisitOperator(n);
        }

        public override SqlNode VisitOrderBy(SqlSelectOrderByClauseNode n)
        {
            return base.VisitOrderBy(n);
        }

        public override SqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            return base.VisitOrderByEntry(n);
        }

        public override SqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
        {
            _result.AssertNotNull(n, nameof(n.Expression), n.Expression);
            return base.VisitParenthesis(n);
        }

        public override SqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            _result.AssertNotNull(n, nameof(n.Operator), n.Operator);
            _result.AssertNotNull(n, nameof(n.Right), n.Right);
            return base.VisitPrefixOperation(n);
        }

        public override SqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            return base.VisitQualifiedIdentifier(n);
        }

        public override SqlNode VisitSelect(SqlSelectNode n)
        {
            // TODO: If the FROM clause contains a SELECT or values subquery, it MUST be in a parenthesis node
            // TODO: If the FROM clause contains a VALUES expression, it MUST be aliased and have ColumnNames
            return base.VisitSelect(n);
        }

        public override SqlNode VisitSet(SqlSetNode n)
        {
            return base.VisitSet(n);
        }

        public override SqlNode VisitStatementList(SqlStatementListNode n)
        {
            return base.VisitStatementList(n);
        }

        public override SqlNode VisitTop(SqlSelectTopNode n)
        {
            return base.VisitTop(n);
        }

        public override SqlNode VisitWith(SqlWithNode n)
        {
            return base.VisitWith(n);
        }

        public override SqlNode VisitWithCte(SqlWithCteNode n)
        {
            // TODO: Detect recursive CTE and validate correct structure
            return base.VisitWithCte(n);
        }

        public override SqlNode VisitUpdate(SqlUpdateNode n)
        {
            return base.VisitUpdate(n);
        }

        public override SqlNode VisitVariable(SqlVariableNode n)
        {
            return base.VisitVariable(n);
        }
    }
}
