using System.Collections.Generic;
using System.Linq;

namespace CastIron.SqlParsing.Ast
{
    public abstract class SqlNodeVisitor
    {
        // TODO: Some kind of ShouldVisit predicate so certain visitors can choose which trees to navigate
        public virtual SqlNode Visit(SqlNode n) => n?.Accept(this);

        public virtual SqlNode VisitAlias(SqlAliasNode n)
        {
            var source = Visit(n.Source);
            var alias = Visit(n.Alias) as SqlIdentifierNode;
            var columns = Visit(n.ColumnNames) as SqlListNode<SqlIdentifierNode>;
            return n.Update(source, alias, columns);
        }

        public virtual SqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            var left = Visit(n.Left);
            var low = Visit(n.Low);
            var high = Visit(n.High);
            return n.Update(n.Not, left, low, high);
        }

        public virtual SqlNode VisitCase(SqlCaseNode n)
        {
            var input = Visit(n.InputExpression);
            var whens = VisitTypedNodeList(n.WhenExpressions);
            var e = Visit(n.ElseExpression);
            return n.Update(input, whens, e);
        }

        public virtual SqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            var cond = Visit(n.Condition);
            var result = Visit(n.Result);
            return n.Update(cond, result);
        }

        public virtual SqlNode VisitCast(SqlCastNode n)
        {
            var expr = Visit(n.Expression);
            var type = Visit(n.DataType) as SqlDataTypeNode;
            return n.Update(expr, type);
        }

        public virtual SqlNode VisitDataType(SqlDataTypeNode n)
        {
            var d = Visit(n.DataType) as SqlKeywordNode;
            var s = Visit(n.Size);
            return n.Update(d, s);
        }

        public virtual SqlNode VisitDeclare(SqlDeclareNode n)
        {
            var v = Visit(n.Variable) as SqlVariableNode;
            var d = Visit(n.DataType);
            var i = Visit(n.Initializer);
            return n.Update(v, d, i);
        }

        public virtual SqlNode VisitDelete(SqlDeleteNode n)
        {
            var source = Visit(n.Source);
            var where = Visit(n.WhereClause);
            return n.Update(source, where);
        }

        public virtual SqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            var name = Visit(n.Name) as SqlIdentifierNode;
            var args = Visit(n.Arguments) as SqlListNode<SqlNode>;
            return n.Update(name, args);
        }

        public virtual SqlNode VisitIdentifier(SqlIdentifierNode n) => n;

        public virtual SqlNode VisitIf(SqlIfNode n)
        {
            var cond = Visit(n.Condition);
            var then = Visit(n.Then);
            var e = Visit(n.Else);
            return n.Update(cond, then, e);
        }

        public virtual SqlNode VisitIn(SqlInNode n)
        {
            var search = Visit(n.Search);
            var items = Visit(n.Items) as SqlListNode<SqlNode>;
            return n.Update(n.Not, search, items);
        }


        public virtual SqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            var l = Visit(n.Left);
            var o = Visit(n.Operator) as SqlOperatorNode;
            var r = Visit(n.Right);
            return n.Update(l, o, r);
        }

        public virtual SqlNode VisitInsert(SqlInsertNode n)
        {
            var table = Visit(n.Table);
            var columns = Visit(n.Columns) as SqlListNode<SqlIdentifierNode>;
            var source = Visit(n.Source);
            return n.Update(table, columns, source);
        }

        public virtual SqlNode VisitValues(SqlValuesNode n)
        {
            var values = Visit(n.Values) as SqlListNode<SqlListNode<SqlNode>>;
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

        public virtual SqlNode VisitKeyword(SqlKeywordNode n) => n;

        public virtual SqlNode VisitList<TNode>(SqlListNode<TNode> n)
            where TNode : SqlNode
        {
            var list = VisitTypedNodeList(n.Children);
            return n.Update(list);
        }

        public virtual SqlNode VisitNull(SqlNullNode n) => n;

        public virtual SqlNode VisitNumber(SqlNumberNode n) => n;

        public virtual SqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
        {
            var server = Visit(n.Server) as SqlIdentifierNode;
            var db = Visit(n.Database) as SqlIdentifierNode; 
            var schema = Visit(n.Schema) as SqlIdentifierNode; 
            var name = Visit(n.Name) as SqlIdentifierNode; 
            return n.Update(server, db, schema, name);
        }

        public virtual SqlNode VisitOperator(SqlOperatorNode n) => n;

        public virtual SqlNode VisitOrderBy(SqlSelectOrderByClauseNode n)
        {
            var entries = Visit(n.Entries) as SqlListNode<SqlOrderByEntryNode>;
            var offset = Visit(n.Offset);
            var limit = Visit(n.Limit);
            return n.Update(entries, offset, limit);
        }

        public virtual SqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            var source = Visit(n.Source);
            return n.Update(source, n.Direction);
        }

        public virtual SqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
            where TNode : SqlNode
        {
            var expr = Visit(n.Expression) as TNode;
            return n.Update(expr);
        }

        public virtual SqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            var op = Visit(n.Operator) as SqlOperatorNode;
            var right = Visit(n.Right);
            return n.Update(op, right);
        }

        public virtual SqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            var qualifier = Visit(n.Qualifier) as SqlIdentifierNode;
            var id = Visit(n.Identifier);
            return n.Update(qualifier, id);
        }

        public virtual SqlNode VisitSelect(SqlSelectNode n)
        {
            var top = Visit(n.TopClause) as SqlSelectTopNode;
            var columns = Visit(n.Columns) as SqlListNode<SqlNode>;
            var from = Visit(n.FromClause);
            var where = Visit(n.WhereClause);
            var orderBy = Visit(n.OrderByClause) as SqlSelectOrderByClauseNode;
            var groupBy = Visit(n.GroupByClause);
            var having = Visit(n.HavingClause);
            return n.Update(n.Modifier, top, columns, from, where, orderBy, groupBy, having);
        }

        public virtual SqlNode VisitSet(SqlSetNode n)
        {
            var v = Visit(n.Variable) as SqlVariableNode;
            var op = Visit(n.Operator) as SqlOperatorNode;
            var r = Visit(n.Right);
            return n.Update(v, op, r);
        }
        
        public virtual SqlNode VisitStatementList(SqlStatementListNode n)
        {
            var stmts = VisitTypedNodeList(n.Statements);
            return n.Update(stmts, n.UseBeginEnd);
        }

        public virtual SqlNode VisitString(SqlStringNode n) => n;

        public virtual SqlNode VisitTop(SqlSelectTopNode n)
        {
            var value = Visit(n.Value);
            return n.Update(value, n.Percent, n.WithTies);
        }

        public virtual SqlNode VisitWith(SqlWithNode n)
        {
            var ctes = Visit(n.Ctes) as SqlListNode<SqlWithCteNode>;
            var stmt = Visit(n.Statement);
            return n.Update(ctes, stmt);
        }

        public virtual SqlNode VisitWithCte(SqlWithCteNode n)
        {
            var name = Visit(n.Name) as SqlIdentifierNode;
            var columns = Visit(n.ColumnNames) as SqlListNode<SqlIdentifierNode>;
            var select = Visit(n.Name);
            return n.Update(name, columns, select);
        }

        public virtual SqlNode VisitUpdate(SqlUpdateNode n)
        {
            var source = Visit(n.Source);
            var sets = Visit(n.SetClause);
            var where = Visit(n.WhereClause);
            return n.Update(source, sets as SqlListNode<SqlInfixOperationNode>, where);
        }

        public virtual SqlNode VisitVariable(SqlVariableNode n) => n;

        protected List<T> VisitTypedNodeList<T>(List<T> list)
            where T : SqlNode
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