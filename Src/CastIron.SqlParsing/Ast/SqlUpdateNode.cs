namespace CastIron.SqlParsing.Ast
{
    public class SqlUpdateNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlListNode<SqlInfixOperationNode> SetClause { get; set; }
        public SqlWhereNode WhereClause { get; set; }

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
                WhereClause.ToString(sb);
            }

            sb.DecreaseIndent();
        }
    }
}