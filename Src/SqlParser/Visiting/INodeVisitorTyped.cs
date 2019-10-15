using SqlParser.Ast;

namespace SqlParser.Visiting
{
    public interface INodeVisitorTyped
    {
        ISqlNode VisitAlias(SqlAliasNode n);
        ISqlNode VisitBetween(SqlBetweenOperationNode n);
        ISqlNode VisitCase(SqlCaseNode n);
        ISqlNode VisitCaseWhen(SqlCaseWhenNode n);
        ISqlNode VisitCast(SqlCastNode n);
        ISqlNode VisitDataType(SqlDataTypeNode n);
        ISqlNode VisitDeclare(SqlDeclareNode n);
        ISqlNode VisitDelete(SqlDeleteNode n);
        ISqlNode VisitExecute(SqlExecuteNode n);
        ISqlNode VisitExecuteArgument(SqlExecuteArgumentNode n);
        ISqlNode VisitFunctionCall(SqlFunctionCallNode n);
        ISqlNode VisitIdentifier(SqlIdentifierNode n);
        ISqlNode VisitIf(SqlIfNode n);
        ISqlNode VisitIn(SqlInNode n);
        ISqlNode VisitInfixOperation(SqlInfixOperationNode n);
        ISqlNode VisitInsert(SqlInsertNode n);
        ISqlNode VisitValues(SqlValuesNode n);
        SqlJoinNode VisitJoin(SqlJoinNode n);
        ISqlNode VisitKeyword(SqlKeywordNode n);

        ISqlNode VisitList<TNode>(SqlListNode<TNode> n)
            where TNode : class, ISqlNode;

        ISqlNode VisitMerge(SqlMergeNode n);
        ISqlNode VisitMergeInsert(SqlMergeInsertNode n);
        ISqlNode VisitMergeUpdate(SqlMergeUpdateNode n);
        ISqlNode VisitNull(SqlNullNode n);
        ISqlNode VisitNumber(SqlNumberNode n);
        ISqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n);
        ISqlNode VisitOperator(SqlOperatorNode n);
        ISqlNode VisitOrderBy(SqlOrderByNode n);
        ISqlNode VisitOrderByEntry(SqlOrderByEntryNode n);
        ISqlNode VisitOver(SqlOverNode n);

        ISqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
            where TNode : class, ISqlNode;

        ISqlNode VisitPrefixOperation(SqlPrefixOperationNode n);
        ISqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n);
        ISqlNode VisitSelect(SqlSelectNode n);
        ISqlNode VisitSet(SqlSetNode n);
        ISqlNode VisitStatementList(SqlStatementListNode n);
        ISqlNode VisitString(SqlStringNode n);
        ISqlNode VisitTopLimit(SqlTopLimitNode n);
        ISqlNode VisitWith(SqlWithNode n);
        ISqlNode VisitWithCte(SqlWithCteNode n);
        ISqlNode VisitUpdate(SqlUpdateNode n);
        ISqlNode VisitVariable(SqlVariableNode n);
    }
}