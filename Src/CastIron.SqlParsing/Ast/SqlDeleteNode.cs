using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlDeleteNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlNode Source { get; set; }
        public SqlNode WhereClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("DELETE FROM ");
            Source.ToString(sb);
            sb.Append(" ");
            if (WhereClause != null)
            {
                sb.AppendLineAndIndent();
                sb.AppendLine("WHERE");
                sb.IncreaseIndent();
                sb.WriteIndent();
                WhereClause.ToString(sb);
                sb.DecreaseIndent();
            }
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitDelete(this);

        public SqlDeleteNode Update(SqlNode source, SqlNode where)
        {
            if (source == Source && where == WhereClause)
                return this;
            return new SqlDeleteNode
            {
                Location = Location,
                Source = source,
                WhereClause = where
            };
        }
    }
}