using System.Linq;
using System.Text;
using SqlParser.Ast;

namespace SqlParser.Stringify
{
    public class SqlServerStringifyVisitor : SqlNodeVisitor
    {
        private readonly StringBuilder _sb;
        private int _indent;

        public SqlServerStringifyVisitor(StringBuilder sb)
        {
            _sb = sb;
            _indent = 0;
        }

        public void AppendLineAndIndent(string s = "")
        {
            AppendLine(s);
            WriteIndent();
        }

        public void AppendLine(string s = "") => _sb.AppendLine(s);
        public void Append(string s) => _sb.Append(s);
        public void WriteIndent()
        {
            if (_indent <= 0)
                return;
            _sb.Append(new string(' ', _indent * 4));
        }
        public void IncreaseIndent() => _indent++;
        public void DecreaseIndent() => _indent--;

        public override SqlNode VisitAlias(SqlAliasNode n)
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

        public override SqlNode VisitBetween(SqlBetweenOperationNode n)
        {
            Visit(n.Left);
            if (n.Not)
                Append(" NOT");
            Append(" BETWEEN ");
            Visit(n.Low);
            Append(" AND ");
            Visit(n.High);
            return n;
        }

        public override SqlNode VisitCase(SqlCaseNode n)
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

        public override SqlNode VisitCaseWhen(SqlCaseWhenNode n)
        {
            Append("WHEN ");
            Visit(n.Condition);
            Append(" THEN ");
            Visit(n.Result);
            return n;
        }

        public override SqlNode VisitCast(SqlCastNode n)
        {
            Append("CAST(");
            Visit(n.Expression);
            Append(" AS ");
            Visit(n.DataType);
            Append(")");
            return n;
        }

        public override SqlNode VisitDataType(SqlDataTypeNode n)
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

        public override SqlNode VisitDeclare(SqlDeclareNode n)
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

        public override SqlNode VisitDelete(SqlDeleteNode n)
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

        public override SqlNode VisitExecute(SqlExecuteNode n)
        {
            Append("EXECUTE ");
            Visit(n.Name);
            Append(" ");
            Visit(n.Arguments);
            return n;
        }

        public override SqlNode VisitExecuteArgument(SqlExecuteArgumentNode n)
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

        public override SqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            Visit(n.Name);
            Append("(");
            if (n.Arguments != null)
                Visit(n.Arguments);
            Append(")");
            return n;
        }

        public override SqlNode VisitIdentifier(SqlIdentifierNode n)
        {
            Append("[");
            Append(n.Name);
            Append("]");
            return n;
        }

        public override SqlNode VisitIf(SqlIfNode n)
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

        public override SqlNode VisitIn(SqlInNode n)
        {
            Visit(n.Search);
            if (n.Not)
                Append(" NOT");
            Append(" IN (");
            Visit(n.Items);
            Append(")");
            return n;
        }

        public override SqlNode VisitInfixOperation(SqlInfixOperationNode n)
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

        public override SqlNode VisitInsert(SqlInsertNode n)
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

        public override SqlNode VisitValues(SqlValuesNode n)
        {
            void forEach(SqlListNode<SqlNode> child)
            {
                Append("(");
                Visit(child);
                Append(")");
            }

            Append("VALUES ");
            forEach(n.Values.First());
            foreach(var child in n.Values.Skip(1))
            {
                Append(", ");
                forEach(child);
            }
            return n;
        }

        public override SqlJoinNode VisitJoin(SqlJoinNode n)
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

        public override SqlNode VisitKeyword(SqlKeywordNode n)
        {
            Append(n.Keyword);
            return n;
        }

        public override SqlNode VisitList<TNode>(SqlListNode<TNode> n)
        {
            if (n.Children.Count == 0)
                return n;
            Visit(n.Children[0]);
            for (int i = 1; i < n.Children.Count; i++)
            {
                Append(", ");
                Visit(n.Children[i]);
            }

            return n;
        }

        public override SqlNode VisitNull(SqlNullNode n)
        {
            Append("NULL");
            return n;
        }

        public override SqlNode VisitNumber(SqlNumberNode n)
        {
            Append(n.ToString());
            return n;
        }

        public override SqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
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

        public override SqlNode VisitOperator(SqlOperatorNode n)
        {
            Append(n.Operator);
            return n;
        }

        public override SqlNode VisitOrderBy(SqlSelectOrderByClauseNode n)
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

        public override SqlNode VisitOrderByEntry(SqlOrderByEntryNode n)
        {
            Visit(n.Source);
            if (!string.IsNullOrEmpty(n.Direction))
            {
                Append(" ");
                Append(n.Direction);
            }

            return n;
        }

        public override SqlNode VisitOver(SqlOverNode n)
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

        public override SqlNode VisitParenthesis<TNode>(SqlParenthesisNode<TNode> n)
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

        public override SqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
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

        public override SqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            if (n.Qualifier != null)
            {
                Visit(n.Qualifier);
                Append(".");
            }

            Visit(n.Identifier);
            return n;
        }

        public override SqlNode VisitSelect(SqlSelectNode n)
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
            Visit(n.Columns);
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

        public override SqlNode VisitSet(SqlSetNode n)
        {
            Append("SET ");
            Visit(n.Variable);
            Append(" ");
            Visit(n.Operator);
            Append(" ");
            Visit(n.Right);
            return n;
        }

        public override SqlNode VisitStatementList(SqlStatementListNode n)
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

        public override SqlNode VisitString(SqlStringNode n)
        {
            Append("'");
            Append(n.Value.Replace("'", "''"));
            Append("'");
            return n;
        }

        public override SqlNode VisitTop(SqlSelectTopNode n)
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

        public override SqlNode VisitWith(SqlWithNode n)
        {
            Append("WITH");

            Visit(n.Ctes);

            Visit(n.Statement);
            return n;
        }

        public override SqlNode VisitWithCte(SqlWithCteNode n)
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

        public override SqlNode VisitUpdate(SqlUpdateNode n)
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

        public override SqlNode VisitVariable(SqlVariableNode n)
        {
            Append(n.Name);
            return n;
        }
    }
}