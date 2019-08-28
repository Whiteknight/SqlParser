namespace SqlParser.Ast
{
    public interface ISqlNodeVisitImplementation
    {
        SqlNode VisitAlias(SqlAliasNode n);
        SqlNode VisitBetween(SqlBetweenOperationNode n);
        SqlNode VisitCase(SqlCaseNode n);
        SqlNode VisitCaseWhen(SqlCaseWhenNode n);
        SqlNode VisitCast(SqlCastNode n);
        SqlNode VisitDataType(SqlDataTypeNode n);
        SqlNode VisitDeclare(SqlDeclareNode n);
        SqlNode VisitDelete(SqlDeleteNode n);
        SqlNode VisitExecute(SqlExecuteNode n);
        SqlNode VisitExecuteArgument(SqlExecuteArgumentNode n);
        SqlNode VisitFunctionCall(SqlFunctionCallNode n);
        SqlNode VisitIdentifier(SqlIdentifierNode n);
        SqlNode VisitIf(SqlIfNode n);
        SqlNode VisitIn(SqlInNode n);
        SqlNode VisitInfixOperation(SqlInfixOperationNode n);
        SqlNode VisitInsert(SqlInsertNode n);
        SqlNode VisitValues(SqlValuesNode n);
        SqlJoinNode VisitJoin(SqlJoinNode n);
        SqlNode VisitKeyword(SqlKeywordNode n);

        SqlNode VisitList<TNode>(SqlListNode<TNode> n)
            where TNode : SqlNode;

        SqlNode VisitMerge(SqlMergeNode n);
        SqlNode VisitMergeInsert(SqlMergeInsertNode n);
        SqlNode VisitMergeUpdate(SqlMergeUpdateNode n);
        SqlNode VisitNull(SqlNullNode n);
        SqlNode VisitNumber(SqlNumberNode n);
        SqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n);
        SqlNode VisitOperator(SqlOperatorNode n);
        SqlNode VisitOrderBy(SqlSelectOrderByClauseNode n);
        SqlNode VisitOrderByEntry(SqlOrderByEntryNode n);
        SqlNode VisitOver(SqlOverNode n);

        SqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
            where TNode : SqlNode;

        SqlNode VisitPrefixOperation(SqlPrefixOperationNode n);
        SqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n);
        SqlNode VisitSelect(SqlSelectNode n);
        SqlNode VisitSet(SqlSetNode n);
        SqlNode VisitStatementList(SqlStatementListNode n);
        SqlNode VisitString(SqlStringNode n);
        SqlNode VisitTop(SqlSelectTopNode n);
        SqlNode VisitWith(SqlWithNode n);
        SqlNode VisitWithCte(SqlWithCteNode n);
        SqlNode VisitUpdate(SqlUpdateNode n);
        SqlNode VisitVariable(SqlVariableNode n);
    }
}