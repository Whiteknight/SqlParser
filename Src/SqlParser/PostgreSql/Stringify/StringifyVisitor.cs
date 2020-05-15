using System;
using System.Linq;
using SqlParser.Ast;
using SqlParser.Visiting;

namespace SqlParser.PostgreSql.Stringify
{
    public partial class StringifyVisitor : INodeVisitorTyped
    {
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
                Append("(", n.Size, ")");

            return n;
        }

        public ISqlNode VisitDeclare(SqlDeclareNode n)
        {
            Append("DECLARE ", n.Variable, " ", n.DataType);
            if (n.Initializer != null)
                Append(" := ", n.Initializer);

            return n;
        }

        public ISqlNode VisitDelete(SqlDeleteNode n)
        {
            Append("DELETE FROM ", n.Source, " ");
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
                Append(n.AssignVariable, " = ");

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
            Append("\"", n.Name, "\"");
            return n;
        }

        public ISqlNode VisitIf(SqlIfNode n)
        {
            Append("IF (", n.Condition, ")");
            AppendLine();
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
                    Append("(", node, ")");
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
            Append("INSERT ");
            if (n.Table != null)
                Append("INTO ", n.Table);

            Append("(", n.Columns, ")");
            IncreaseIndent();
            if (n.Source != null)
            {
                WriteIndent();
                Visit(n.Source);
            }
            if (n.OnConflict != null)
            {
                AppendLineAndIndent();
                Append("ON CONFLICT DO ", n.OnConflict);
            }

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
            Append("USING ", n.Source);
            AppendLineAndIndent();
            Append("ON ", n.MergeCondition);
            foreach (var matchClause in n.MatchClauses)
            {
                AppendLineAndIndent();
                Visit(matchClause.Keyword);
                // TODO: Condition
                Append(" THEN");
                IncreaseIndent();
                AppendLineAndIndent();
                Visit(matchClause.Action);
                DecreaseIndent();
            }

            DecreaseIndent();

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
                Append(n.Server, ".");

            if (n.Database != null)
                Append(n.Database, ".");

            if (n.Schema != null)
                Append(n.Schema, ".");

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
                Append(" ", n.Direction);

            return n;
        }

        public ISqlNode VisitOver(SqlOverNode n)
        {
            Visit(n.Expression);
            if (n.PartitionBy == null && n.OrderBy == null && n.RowsRange == null)
                return n;
            Append(" OVER (");
            if (n.PartitionBy != null)
                Append("PARTITION BY ", n.PartitionBy);
            if (n.OrderBy != null)
                Append(" ORDER BY ", n.OrderBy);
            if (n.RowsRange != null)
                Append(" ROWS ", n.RowsRange);

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
            Append(n.Operator, " ");
            if (n.Right is SqlInfixOperationNode)
                Append("(", n.Right, ")");
            else
                Visit(n.Right);

            return n;
        }

        public ISqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            if (n.Qualifier != null)
                Append(n.Qualifier, ".");

            Visit(n.Identifier);
            return n;
        }

        public ISqlNode VisitSelect(SqlSelectNode n)
        {
            Append("SELECT ");
            if (n.Modifier != null)
                Append(n.Modifier, " ");

            IncreaseIndent();
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
            if (n.TopLimitClause != null)
            {
                AppendLineAndIndent();
                Visit(n.TopLimitClause);
            }

            DecreaseIndent();
            return n;
        }

        public ISqlNode VisitSet(SqlSetNode n)
        {
            if (n.Operator.Operator == ":=" || n.Operator.Operator == "=")
                Append(n.Variable, " := ", n.Right);
            else if (n.Operator.Operator.Length == 2 && n.Operator.Operator[1] == '=')
                Append(n.Variable, " := ", n.Variable, n.Operator.Operator[0].ToString(), n.Right);
            else
                Append(n.Variable, " := /* ERROR: Operator ", n.Operator, " unsupported in PostgreSQL */", n.Right);

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
            Append(n.ToString());
            return n;
        }

        public ISqlNode VisitTopLimit(SqlTopLimitNode n)
        {
            Append("LIMIT ", n.Value);
            return n;
        }

        public ISqlNode VisitValues(SqlValuesNode n)
        {
            void forEach(SqlListNode<ISqlNode> child) => Append("(", child, ")");

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
            if (n.Recursive)
                Append("RECURSIVE ");
            Visit(n.Name);
            if (n.ColumnNames != null && n.ColumnNames.Any())
                Append("(", n.ColumnNames, ")");
            Append(" AS (");
            IncreaseIndent();
            AppendLineAndIndent();
            Visit(n.Select);
            AppendLineAndIndent();
            DecreaseIndent();
            Append(")");
            return n;
        }

        public ISqlNode VisitUnknown(SqlUnknownStatementNode n)
        {
            Append(n.ToString());
            return n;
        }

        public ISqlNode VisitUpdate(SqlUpdateNode n)
        {
            Append("UPDATE ");
            if (n.Source != null)
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