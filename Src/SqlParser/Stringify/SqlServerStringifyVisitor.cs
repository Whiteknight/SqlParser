using System;
using System.Linq;
using SqlParser.Ast;

namespace SqlParser.Stringify
{
    public partial class SqlServerStringifyVisitor : ISqlNodeVisitor, ISqlNodeVisitImplementation
    {
        public SqlNode Visit(SqlNode n) => n?.Accept(this);

        public SqlNode VisitAlias(SqlAliasNode n)
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

        public SqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            Append(n.Left, n.Not ? " NOT" : "", " BETWEEN ", n.Low, " AND ", n.High);
            return n;
        }

        public SqlNode VisitCase(SqlCaseNode n)
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

        public SqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            Append("WHEN ", n.Condition, " THEN ", n.Result);
            return n;
        }

        public SqlNode VisitCast(SqlCastNode n)
        {
            Append("CAST(", n.Expression, " AS ", n.DataType, ")");
            return n;
        }

        public SqlNode VisitDataType(SqlDataTypeNode n)
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

        public SqlNode VisitDeclare(SqlDeclareNode n)
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

        public SqlNode VisitDelete(SqlDeleteNode n)
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

        public SqlNode VisitExecute(SqlExecuteNode n)
        {
            Append("EXECUTE ", n.Name, " ", n.Arguments);
            return n;
        }

        public SqlNode VisitExecuteArgument(SqlExecuteArgumentNode n)
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

        public SqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            Append(n.Name, "(", n.Arguments, ")");
            return n;
        }

        public SqlNode VisitIdentifier(SqlIdentifierNode n)
        {
            Append("[", n.Name, "]");
            return n;
        }

        public SqlNode VisitIf(SqlIfNode n)
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

        public SqlNode VisitIn(SqlInNode n)
        {
            Append(n.Search, n.Not ? " NOT" : "", " IN (", n.Items, ")");
            return n;
        }

        public SqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            void ToStringChild(SqlNode node)
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

        public SqlNode VisitInsert(SqlInsertNode n)
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

        public SqlNode VisitKeyword(SqlKeywordNode n)
        {
            Append(n.Keyword);
            return n;
        }

        public SqlNode VisitList<TNode>(SqlListNode<TNode> n) 
            where TNode : SqlNode 
            => VisitList(n, () => Append(", "));

        private SqlNode VisitList<TNode>(SqlListNode<TNode> n, Action between)
            where TNode : SqlNode
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

        public SqlNode VisitMerge(SqlMergeNode n)
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

        public SqlNode VisitMergeInsert(SqlMergeInsertNode n)
        {
            Append("INSERT (", n.Columns, ") ", n.Source);
            return n;
        }

        public SqlNode VisitMergeUpdate(SqlMergeUpdateNode n)
        {
            Append("UPDATE SET ", n.SetClause);
            return n;
        }

        public SqlNode VisitNull(SqlNullNode n)
        {
            Append("NULL");
            return n;
        }

        public SqlNode VisitNumber(SqlNumberNode n)
        {
            Append(n.ToString());
            return n;
        }

        public SqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
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

        public SqlNode VisitOperator(SqlOperatorNode n)
        {
            Append(n.Operator);
            return n;
        }

        public SqlNode VisitOrderBy(SqlSelectOrderByClauseNode n)
        {
            Append("ORDER BY");
            IncreaseIndent();
            AppendLineAndIndent();
            Visit(n.Entries);
            if (n.Offset != null || n.Limit != null)
            {
                AppendLineAndIndent();
                if (n.Offset != null)
                {
                    Append("OFFSET ");
                    Visit(n.Offset);
                    Append(" ROWS ");
                }

                if (n.Limit != null)
                {
                    Append("FETCH NEXT ");
                    Visit(n.Limit);
                    Append(" ROWS ONLY");
                }
            }

            return n;
        }

        public SqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            Visit(n.Source);
            if (!string.IsNullOrEmpty(n.Direction))
            {
                Append(" ");
                Append(n.Direction);
            }

            return n;
        }

        public SqlNode VisitOver(SqlOverNode n)
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

        public SqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n) 
            where TNode : SqlNode
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

        public SqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
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

        public SqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            if (n.Qualifier != null)
            {
                Visit(n.Qualifier);
                Append(".");
            }

            Visit(n.Identifier);
            return n;
        }

        public SqlNode VisitSelect(SqlSelectNode n)
        {
            Append("SELECT ");
            if (n.Modifier != null)
            {
                Append(n.Modifier);
                Append(" ");
            }

            IncreaseIndent();

            if (n.TopClause != null)
            {
                AppendLineAndIndent();
                Visit(n.TopClause);
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

            if (n.OrderByClause != null)
            {
                AppendLineAndIndent();
                Visit(n.OrderByClause);
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

            DecreaseIndent();
            return n;
        }

        public SqlNode VisitSet(SqlSetNode n)
        {
            Append("SET ", n.Variable, " ", n.Operator, " ", n.Right);
            return n;
        }

        public SqlNode VisitStatementList(SqlStatementListNode n)
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

        public SqlNode VisitString(SqlStringNode n)
        {
            Append("'", n.Value.Replace("'", "''"), "'");
            return n;
        }

        public SqlNode VisitTop(SqlSelectTopNode n)
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

        public SqlNode VisitValues(SqlValuesNode n)
        {
            void forEach(SqlListNode<SqlNode> child)
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

        public SqlNode VisitWith(SqlWithNode n)
        {
            Append("WITH ", n.Ctes, n.Statement);
            return n;
        }

        public SqlNode VisitWithCte(SqlWithCteNode n)
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

        public SqlNode VisitUpdate(SqlUpdateNode n)
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

        public SqlNode VisitVariable(SqlVariableNode n)
        {
            Append(n.Name);
            return n;
        }
    }
}