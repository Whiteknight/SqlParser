using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlUpdateNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlNode Source { get; set; }
        public SqlListNode<SqlInfixOperationNode> SetClause { get; set; }
        public SqlNode WhereClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("UPDATE ");
            Source.ToString(sb);
            sb.IncreaseIndent();
            sb.AppendLineAndIndent();
            sb.AppendLine("SET");
            sb.IncreaseIndent();

            void forEach(SqlStringifier x, SqlInfixOperationNode child)
            {
                sb.WriteIndent();
                child.ToString(x);
            }
            void between(SqlStringifier x)
            {
                x.AppendLineAndIndent(",");
            }

            SetClause.ToString(sb, forEach, between);
            sb.DecreaseIndent();
            if (WhereClause != null)
            {
                sb.AppendLineAndIndent();
                sb.AppendLine("WHERE");
                sb.IncreaseIndent();
                sb.WriteIndent();
                WhereClause.ToString(sb);
                sb.DecreaseIndent();
            }

            sb.DecreaseIndent();
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitUpdate(this);

        public SqlUpdateNode Update(SqlNode source, SqlListNode<SqlInfixOperationNode> set, SqlNode where)
        {
            if (source == Source && set == SetClause && where == WhereClause)
                return this;
            return new SqlUpdateNode
            {
                Location = Location,
                Source = source,
                SetClause = set,
                WhereClause = where
            };
        }
    }
}