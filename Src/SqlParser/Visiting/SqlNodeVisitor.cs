using System.Collections.Generic;
using System.Linq;
using SqlParser.Ast;

namespace SqlParser.Visiting
{
    public abstract class SqlNodeVisitor : ISqlNodeVisitor, INodeVisitorTyped
    {
        // TODO: Some kind of ShouldVisit predicate so certain visitors can choose which trees to navigate
        public virtual ISqlNode Visit(ISqlNode n) => (n as ISqlNode)?.Accept(this);

        public virtual ISqlNode VisitAlias(SqlAliasNode n)
        {
            var source = Visit(n.Source);
            var alias = Visit(n.Alias) as SqlIdentifierNode;
            var columns = Visit(n.ColumnNames) as SqlListNode<SqlIdentifierNode>;
            return n.Update(source, alias, columns);
        }

        public virtual ISqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            var left = Visit(n.Left);
            var low = Visit(n.Low);
            var high = Visit(n.High);
            return n.Update(n.Not, left, low, high);
        }

        public virtual ISqlNode VisitCase(SqlCaseNode n)
        {
            var input = Visit(n.InputExpression);
            var whens = VisitTypedNodeList(n.WhenExpressions);
            var e = Visit(n.ElseExpression);
            return n.Update(input, whens, e);
        }

        public virtual ISqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            var cond = Visit(n.Condition);
            var result = Visit(n.Result);
            return n.Update(cond, result);
        }

        public virtual ISqlNode VisitCast(SqlCastNode n)
        {
            var expr = Visit(n.Expression);
            var type = Visit(n.DataType) as SqlDataTypeNode;
            return n.Update(expr, type);
        }

        public virtual ISqlNode VisitDataType(SqlDataTypeNode n)
        {
            var d = Visit(n.DataType) as SqlKeywordNode;
            var s = Visit(n.Size);
            return n.Update(d, s);
        }

        public virtual ISqlNode VisitDeclare(SqlDeclareNode n)
        {
            var v = Visit(n.Variable) as SqlVariableNode;
            var d = Visit(n.DataType);
            var i = Visit(n.Initializer);
            return n.Update(v, d, i);
        }

        public virtual ISqlNode VisitDelete(SqlDeleteNode n)
        {
            var source = Visit(n.Source);
            var where = Visit(n.WhereClause);
            return n.Update(source, where);
        }

        public virtual ISqlNode VisitExecute(SqlExecuteNode n)
        {
            var name = Visit(n.Name);
            var args = Visit(n.Arguments) as SqlListNode<SqlExecuteArgumentNode>;
            return n.Update(name, args);
        }

        public virtual ISqlNode VisitExecuteArgument(SqlExecuteArgumentNode n)
        {
            var assign = Visit(n.AssignVariable) as SqlVariableNode;
            var value = Visit(n.Value);
            return n.Update(assign, value, n.IsOut);
        }

        public virtual ISqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            var name = Visit(n.Name);
            var args = Visit(n.Arguments) as SqlListNode<ISqlNode>;
            return n.Update(name, args);
        }

        public virtual ISqlNode VisitIdentifier(SqlIdentifierNode n) => n;

        public virtual ISqlNode VisitIf(SqlIfNode n)
        {
            var cond = Visit(n.Condition);
            var then = Visit(n.Then);
            var e = Visit(n.Else);
            return n.Update(cond, then, e);
        }

        public virtual ISqlNode VisitIn(SqlInNode n)
        {
            var search = Visit(n.Search);
            var items = Visit(n.Items) as SqlListNode<ISqlNode>;
            return n.Update(n.Not, search, items);
        }

        public virtual ISqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            var l = Visit(n.Left);
            var o = Visit(n.Operator) as SqlOperatorNode;
            var r = Visit(n.Right);
            return n.Update(l, o, r);
        }

        public virtual ISqlNode VisitInsert(SqlInsertNode n)
        {
            var table = Visit(n.Table);
            var columns = Visit(n.Columns) as SqlListNode<SqlIdentifierNode>;
            var source = Visit(n.Source);
            return n.Update(table, columns, source);
        }

        public virtual ISqlNode VisitValues(SqlValuesNode n)
        {
            var values = Visit(n.Values) as SqlListNode<SqlListNode<ISqlNode>>;
            return n.Update(values);
        }

        public virtual SqlJoinNode VisitJoin(SqlJoinNode n)
        {
            var left = Visit(n.Left);
            var op = Visit(n.Operator) as SqlOperatorNode;
            var right = Visit(n.Right);
            var cond = Visit(n.OnCondition);
            return n.Update(left, op, right, cond);
        }

        public virtual ISqlNode VisitKeyword(SqlKeywordNode n) => n;

        public virtual ISqlNode VisitList<TNode>(SqlListNode<TNode> n)
            where TNode : class, ISqlNode
        {
            var list = VisitTypedNodeList(n.Children);
            return n.Update(list);
        }

        public virtual ISqlNode VisitMerge(SqlMergeNode n)
        {
            var target = Visit(n.Target);
            var source = Visit(n.Source);
            var cond = Visit(n.MergeCondition);
            var m = Visit(n.Matched);
            var nmt = Visit(n.NotMatchedByTarget) as SqlMergeInsertNode;
            var nms = Visit(n.NotMatchedBySource);
            return n.Update(target, source, cond, m, nmt, nms);
        }

        public virtual ISqlNode VisitMergeInsert(SqlMergeInsertNode n)
        {
            var cols = Visit(n.Columns) as SqlListNode<SqlIdentifierNode>;
            var source = Visit(n.Source);
            return n.Update(cols, source);
        }

        public virtual ISqlNode VisitMergeUpdate(SqlMergeUpdateNode n)
        {
            var cols = Visit(n.SetClause) as SqlListNode<SqlInfixOperationNode>;
            return n.Update(cols);
        }

        public virtual ISqlNode VisitNull(SqlNullNode n) => n;

        public virtual ISqlNode VisitNumber(SqlNumberNode n) => n;

        public virtual ISqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
        {
            var server = Visit(n.Server) as SqlIdentifierNode;
            var db = Visit(n.Database) as SqlIdentifierNode; 
            var schema = Visit(n.Schema) as SqlIdentifierNode; 
            var name = Visit(n.Name) as SqlIdentifierNode; 
            return n.Update(server, db, schema, name);
        }

        public virtual ISqlNode VisitOperator(SqlOperatorNode n) => n;

        public virtual ISqlNode VisitOrderBy(SqlOrderByNode n)
        {
            var entries = Visit(n.Entries) as SqlListNode<SqlOrderByEntryNode>;
            return n.Update(entries);
        }

        public virtual ISqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            var source = Visit(n.Source);
            return n.Update(source, n.Direction);
        }

        public virtual ISqlNode VisitOver(SqlOverNode n)
        {
            var e = Visit(n.Expression);
            var p = Visit(n.PartitionBy);
            var o = Visit(n.OrderBy);
            var r = Visit(n.RowsRange);
            return n.Update(e, p, o, r);
        }

        public virtual ISqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
            where TNode : class, ISqlNode
        {
            var expr = Visit(n.Expression) as TNode;
            return n.Update(expr);
        }

        public virtual ISqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            var op = Visit(n.Operator) as SqlOperatorNode;
            var right = Visit(n.Right);
            return n.Update(op, right);
        }

        public virtual ISqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            var qualifier = Visit(n.Qualifier) as SqlIdentifierNode;
            var id = Visit(n.Identifier);
            return n.Update(qualifier, id);
        }

        public virtual ISqlNode VisitSelect(SqlSelectNode n)
        {
            var top = Visit(n.TopLimitClause) as SqlTopLimitNode;
            var columns = Visit(n.Columns) as SqlListNode<ISqlNode>;
            var from = Visit(n.FromClause);
            var where = Visit(n.WhereClause);
            var orderBy = Visit(n.OrderByClause) as SqlOrderByNode;
            var groupBy = Visit(n.GroupByClause);
            var having = Visit(n.HavingClause);
            var offset = Visit(n.OffsetClause);
            var fetch = Visit(n.FetchClause);
            return n.Update(n.Modifier, top, columns, from, where, orderBy, groupBy, having, offset, fetch);
        }

        public virtual ISqlNode VisitSet(SqlSetNode n)
        {
            var v = Visit(n.Variable) as SqlVariableNode;
            var op = Visit(n.Operator) as SqlOperatorNode;
            var r = Visit(n.Right);
            return n.Update(v, op, r);
        }
        
        public virtual ISqlNode VisitStatementList(SqlStatementListNode n)
        {
            var stmts = VisitTypedNodeList(n.Statements);
            return n.Update(stmts, n.UseBeginEnd);
        }

        public virtual ISqlNode VisitString(SqlStringNode n) => n;

        public virtual ISqlNode VisitTopLimit(SqlTopLimitNode n)
        {
            var value = Visit(n.Value);
            return n.Update(value, n.Percent, n.WithTies);
        }

        public virtual ISqlNode VisitWith(SqlWithNode n)
        {
            var ctes = Visit(n.Ctes) as SqlListNode<SqlWithCteNode>;
            var stmt = Visit(n.Statement);
            return n.Update(ctes, stmt);
        }

        public virtual ISqlNode VisitWithCte(SqlWithCteNode n)
        {
            var name = Visit(n.Name) as SqlIdentifierNode;
            var columns = Visit(n.ColumnNames) as SqlListNode<SqlIdentifierNode>;
            var select = Visit(n.Name);
            return n.Update(name, columns, select, n.Recursive);
        }

        public virtual ISqlNode VisitUpdate(SqlUpdateNode n)
        {
            var source = Visit(n.Source);
            var sets = Visit(n.SetClause);
            var where = Visit(n.WhereClause);
            return n.Update(source, sets as SqlListNode<SqlInfixOperationNode>, where);
        }

        public virtual ISqlNode VisitVariable(SqlVariableNode n) => n;

        protected List<T> VisitTypedNodeList<T>(List<T> list)
            where T : class, ISqlNode
        {
            T[] newNodes = null;
            for (int i = 0; i < list.Count; i++)
            {
                var oldNode = list[i];
                var newNode = Visit(oldNode) as T;
                if (newNode != oldNode)
                {
                    newNodes = new T[list.Count];
                    for (int j = 0; j < i; j++)
                        newNodes[j] = list[j];
                }

                if (newNodes != null)
                    newNodes[i] = newNode;
            }

            return newNodes?.ToList() ?? list;
        }
    }
}