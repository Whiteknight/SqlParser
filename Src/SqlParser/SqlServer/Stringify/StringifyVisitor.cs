using System;
using System.Linq;
using SqlParser.Ast;
using SqlParser.Visiting;

namespace SqlParser.SqlServer.Stringify
{
    public partial class StringifyVisitor : ISqlNodeVisitor, INodeVisitorTyped
    {
        public ISqlNode Visit(ISqlNode n) => n?.Accept(this);

        public ISqlNode VisitAlias(SqlAliasNode n)
        {
            Visit(n.Source);
            Append(" AS ");
            Visit(n.Alias);
            if (n.ColumnNames != null && n.ColumnNames.Any())
            {
                Append("(");
                Visit(n.ColumnNames);
                Append(")");
            }
            return n;
        }

        public ISqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            Append(n.Left, n.Not ? " NOT" : "", " BETWEEN ", n.Low, " AND ", n.High);
            return n;
        }

        public ISqlNode VisitCase(SqlCaseNode n)
        {
            Append("CASE ");
            Visit(n.InputExpression);
            IncreaseIndent();

            foreach (var when in n.WhenExpressions)
            {
                AppendLineAndIndent();
                Visit(when);
            }
            if (n.ElseExpression != null)
            {
                AppendLineAndIndent();
                Append("ELSE ");
                Visit(n.ElseExpression);
            }

            DecreaseIndent();
            AppendLineAndIndent();
            Append("END");
            return n;
        }

        public ISqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            Append("WHEN ", n.Condition, " THEN ", n.Result);
            return n;
        }

        public ISqlNode VisitCast(SqlCastNode n)
        {
            Append("CAST(", n.Expression, " AS ", n.DataType, ")");
            return n;
        }

        public ISqlNode VisitDataType(SqlDataTypeNode n)
        {
            Visit(n.DataType);
            if (n.Size != null)
            {
                Append("(");
                Visit(n.Size);
                Append(")");
            }

            return n;
        }

        public ISqlNode VisitDeclare(SqlDeclareNode n)
        {
            Append("DECLARE ");
            Visit(n.Variable);
            Append(" ");
            Visit(n.DataType);
            if (n.Initializer != null)
            {
                Append(" = ");
                Visit(n.Initializer);
            }

            return n;
        }

        public ISqlNode VisitDelete(SqlDeleteNode n)
        {
            Append("DELETE FROM ");
            Visit(n.Source);
            Append(" ");
            if (n.WhereClause != null)
            {
                AppendLineAndIndent();
                AppendLine("WHERE");
                IncreaseIndent();
                WriteIndent();
                Visit(n.WhereClause);
                DecreaseIndent();
            }

            return n;
        }

        public ISqlNode VisitExecute(SqlExecuteNode n)
        {
            Append("EXECUTE ", n.Name, " ", n.Arguments);
            return n;
        }

        public ISqlNode VisitExecuteArgument(SqlExecuteArgumentNode n)
        {
            if (n.AssignVariable != null)
            {
                Visit(n.AssignVariable);
                Append(" = ");
            }

            Visit(n.Value);
            if (n.IsOut)
                Append(" OUTPUT");
            return n;
        }

        public ISqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            Append(n.Name, "(", n.Arguments, ")");
            return n;
        }

        public ISqlNode VisitIdentifier(SqlIdentifierNode n)
        {
            Append("[", n.Name, "]");
            return n;
        }

        public ISqlNode VisitIf(SqlIfNode n)
        {
            Append("IF (");
            Visit(n.Condition);
            AppendLine(")");
            IncreaseIndent();
            Visit(n.Then);
            if (!(n.Then is SqlStatementListNode))
                AppendLine(";");
            AppendLine();
            DecreaseIndent();
            if (n.Else != null)
            {
                AppendLine("ELSE");
                IncreaseIndent();
                Visit(n.Else);
                if (!(n.Else is SqlStatementListNode))
                    AppendLine(";");
                AppendLine();
                DecreaseIndent();
            }

            return n;
        }

        public ISqlNode VisitIn(SqlInNode n)
        {
            Append(n.Search, n.Not ? " NOT" : "", " IN (", n.Items, ")");
            return n;
        }

        public ISqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            void ToStringChild(ISqlNode node)
            {
                if (node is SqlInfixOperationNode)
                {
                    Append("(");
                    Visit(node);
                    Append(")");
                    return;
                }

                Visit(node);
            }
            if (n.IsUnionOperation())
            {
                Visit(n.Left);
                AppendLineAndIndent();
                Visit(n.Operator);
                AppendLineAndIndent();
                Visit(n.Right);
            }
            else if (n.IsBooleanOperation())
            {
                ToStringChild(n.Left);
                AppendLineAndIndent();
                Visit(n.Operator);
                Append(" ");
                ToStringChild(n.Right);
            }
            else
            {
                ToStringChild(n.Left);
                Append(" ");
                Visit(n.Operator);
                Append(" ");
                ToStringChild(n.Right);
            }
            
            return n;
        }

        public ISqlNode VisitInsert(SqlInsertNode n)
        {
            Append("INSERT INTO ");
            Visit(n.Table);
            Append("(");
            Visit(n.Columns);
            AppendLine(")");
            IncreaseIndent();
            WriteIndent();
            Visit(n.Source);
            DecreaseIndent();
            return n;
        }

        public SqlJoinNode VisitJoin(SqlJoinNode n)
        {
            Visit(n.Left);
            AppendLineAndIndent();
            Visit(n.Operator);
            AppendLineAndIndent();
            Visit(n.Right);

            if (n.OnCondition != null)
            {
                IncreaseIndent();
                AppendLineAndIndent();
                Append("ON ");
                Visit(n.OnCondition);
                DecreaseIndent();
            }

            return n;
        }

        public ISqlNode VisitKeyword(SqlKeywordNode n)
        {
            Append(n.Keyword);
            return n;
        }

        public ISqlNode VisitList<TNode>(SqlListNode<TNode> n) 
            where TNode : class, ISqlNode 
            => VisitList(n, () => Append(", "));

        private ISqlNode VisitList<TNode>(SqlListNode<TNode> n, Action between)
            where TNode : class, ISqlNode
        {
            if (n.Children.Count == 0)
                return n;
            Visit(n.Children[0]);
            for (int i = 1; i < n.Children.Count; i++)
            {
                between?.Invoke();
                Visit(n.Children[i]);
            }

            return n;
        }

        public ISqlNode VisitMerge(SqlMergeNode n)
        {
            Append("MERGE");
            IncreaseIndent();
            AppendLineAndIndent();
            Visit(n.Target);
            AppendLineAndIndent();
            Append("USING ");
            Visit(n.Source);
            AppendLineAndIndent();
            Append("ON ");
            Visit(n.MergeCondition);
            if (n.Matched != null)
            {
                AppendLineAndIndent();
                Append("WHEN MATCHED ");
                Append(" THEN");
                IncreaseIndent();
                AppendLineAndIndent();
                Visit(n.Matched);
                DecreaseIndent();
            }
            if (n.NotMatchedByTarget != null)
            {
                AppendLineAndIndent();
                Append("WHEN NOT MATCHED BY TARGET ");
                Append(" THEN");
                IncreaseIndent();
                AppendLineAndIndent();
                Visit(n.NotMatchedByTarget);
                DecreaseIndent();
            }
            if (n.NotMatchedBySource != null)
            {
                AppendLineAndIndent();
                Append("WHEN NOT MATCHED BY SOURCE");
                Append(" THEN");
                IncreaseIndent();
                AppendLineAndIndent();
                Visit(n.NotMatchedBySource);
                DecreaseIndent();
            }

            DecreaseIndent();

            return n;
        }

        public ISqlNode VisitMergeInsert(SqlMergeInsertNode n)
        {
            Append("INSERT (", n.Columns, ") ", n.Source);
            return n;
        }

        public ISqlNode VisitMergeUpdate(SqlMergeUpdateNode n)
        {
            Append("UPDATE SET ", n.SetClause);
            return n;
        }

        public ISqlNode VisitNull(SqlNullNode n)
        {
            Append("NULL");
            return n;
        }

        public ISqlNode VisitNumber(SqlNumberNode n)
        {
            Append(n.ToString());
            return n;
        }

        public ISqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
        {
            if (n.Server != null)
            {
                Visit(n.Server);
                Append(".");
            }

            if (n.Database != null)
            {
                Visit(n.Database);
                Append(".");
            }

            if (n.Schema != null)
            {
                Visit(n.Schema);
                Append(".");
            }

            Visit(n.Name);
            return n;
        }

        public ISqlNode VisitOperator(SqlOperatorNode n)
        {
            Append(n.Operator);
            return n;
        }

        public ISqlNode VisitOrderBy(SqlOrderByNode n)
        {
            Append("ORDER BY");
            IncreaseIndent();
            AppendLineAndIndent();
            Visit(n.Entries);
            return n;
        }

        public ISqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            Visit(n.Source);
            if (!string.IsNullOrEmpty(n.Direction))
            {
                Append(" ");
                Append(n.Direction);
            }

            return n;
        }

        public ISqlNode VisitOver(SqlOverNode n)
        {
            Visit(n.Expression);
            if (n.PartitionBy == null && n.OrderBy == null && n.RowsRange == null)
                return n;
            Append(" OVER (");
            if (n.PartitionBy != null)
            {
                Append("PARTITION BY ");
                Visit(n.PartitionBy);
            }
            if (n.OrderBy != null)
            {
                Append(" ORDER BY ");
                Visit(n.OrderBy);
            }
            if (n.RowsRange != null)
            {
                Append(" ROWS ");
                Visit(n.RowsRange);
            }

            Append(")");
            return n;
        }

        public ISqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n) 
            where TNode : class, ISqlNode
        {
            AppendLine("(");
            IncreaseIndent();
            WriteIndent();
            if (n.Expression != null)
                Visit(n.Expression);
            DecreaseIndent();
            AppendLineAndIndent();
            Append(")");
            return n;
        }

        public ISqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            Visit(n.Operator);
            Append(" ");
            if (n.Right is SqlInfixOperationNode)
            {
                Append("(");
                Visit(n.Right);
                Append(")");
            }
            else
                Visit(n.Right);

            return n;
        }

        public ISqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            if (n.Qualifier != null)
            {
                Visit(n.Qualifier);
                Append(".");
            }

            Visit(n.Identifier);
            return n;
        }

        public ISqlNode VisitSelect(SqlSelectNode n)
        {
            Append("SELECT ");
            if (n.Modifier != null)
            {
                Append(n.Modifier);
                Append(" ");
            }

            IncreaseIndent();

            if (n.TopLimitClause != null)
            {
                AppendLineAndIndent();
                Visit(n.TopLimitClause);
            }
            AppendLineAndIndent();
            VisitList(n.Columns, () => AppendLineAndIndent(","));
            if (n.FromClause != null)
            {
                AppendLineAndIndent();
                AppendLine("FROM ");
                IncreaseIndent();
                WriteIndent();
                Visit(n.FromClause);
                DecreaseIndent();
            }

            if (n.WhereClause != null)
            {
                AppendLineAndIndent();
                AppendLine("WHERE");
                IncreaseIndent();
                WriteIndent();
                Visit(n.WhereClause);
                DecreaseIndent();
            }
            if (n.GroupByClause != null)
            {
                AppendLineAndIndent();
                Append("GROUP BY");
                IncreaseIndent();
                AppendLineAndIndent();
                Visit(n.GroupByClause);
                DecreaseIndent();
            }
            if (n.HavingClause != null)
            {
                AppendLineAndIndent();
                AppendLine("HAVING");
                IncreaseIndent();
                WriteIndent();
                Visit(n.HavingClause);
                DecreaseIndent();
            }
            if (n.OrderByClause != null)
            {
                AppendLineAndIndent();
                Visit(n.OrderByClause);
            }

            if (n.OffsetClause != null)
            {
                AppendLineAndIndent();
                Append("OFFSET ", n.OffsetClause, " ROWS");
            }

            if (n.FetchClause != null)
            {
                AppendLineAndIndent();
                Append("FETCH NEXT ", n.FetchClause, " ROWS ONLY");
            }

            DecreaseIndent();
            return n;
        }

        public ISqlNode VisitSet(SqlSetNode n)
        {
            Append("SET ", n.Variable, " ", n.Operator, " ", n.Right);
            return n;
        }

        public ISqlNode VisitStatementList(SqlStatementListNode n)
        {
            if (n.UseBeginEnd)
            {
                WriteIndent();
                AppendLine("BEGIN");
                IncreaseIndent();
            }

            foreach (var statement in n.Statements)
            {
                WriteIndent();
                Visit(statement);
                if (!(statement is SqlStatementListNode || statement is SqlIfNode))
                    AppendLine(";");
            }

            if (n.UseBeginEnd)
            {
                DecreaseIndent();
                WriteIndent();
                AppendLine("END");
            }

            return n;
        }

        public ISqlNode VisitString(SqlStringNode n)
        {
            Append("'", n.Value.Replace("'", "''"), "'");
            return n;
        }

        public ISqlNode VisitTopLimit(SqlTopLimitNode n)
        {
            Append("TOP (");
            Visit(n.Value);
            Append(")");
            if (n.Percent)
                Append(" PERCENT");
            if (n.WithTies)
                Append(" WITH TIES");
            return n;
        }

        public ISqlNode VisitValues(SqlValuesNode n)
        {
            void forEach(SqlListNode<ISqlNode> child)
            {
                Append("(");
                Visit(child);
                Append(")");
            }

            Append("VALUES ");
            forEach(n.Values.First());
            foreach (var child in n.Values.Skip(1))
            {
                Append(", ");
                forEach(child);
            }
            return n;
        }

        public ISqlNode VisitWith(SqlWithNode n)
        {
            Append("WITH ", n.Ctes, n.Statement);
            return n;
        }

        public ISqlNode VisitWithCte(SqlWithCteNode n)
        {
            Visit(n.Name);
            if (n.ColumnNames != null && n.ColumnNames.Any())
            {
                Append("(");
                Visit(n.ColumnNames);
                Append(")");
            }
            Append(" AS (");
            IncreaseIndent();
            AppendLineAndIndent();
            Visit(n.Select);
            AppendLineAndIndent();
            DecreaseIndent();
            Append(")");
            return n;
        }

        public ISqlNode VisitUpdate(SqlUpdateNode n)
        {
            Append("UPDATE ");
            Visit(n.Source);
            IncreaseIndent();
            AppendLineAndIndent();
            AppendLine("SET");
            IncreaseIndent();

            Visit(n.SetClause);
            DecreaseIndent();
            if (n.WhereClause != null)
            {
                AppendLineAndIndent();
                AppendLine("WHERE");
                IncreaseIndent();
                WriteIndent();
                Visit(n.WhereClause);
                DecreaseIndent();
            }

            DecreaseIndent();
            return n;
        }

        public ISqlNode VisitVariable(SqlVariableNode n)
        {
            Append(n.Name);
            return n;
        }
    }
}