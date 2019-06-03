namespace CastIron.SqlParsing.Ast
{
    public class SqlWithNode : SqlNode
    {
        public SqlListNode<SqlCteNode> Ctes { get; set; }
        public SqlNode Statement { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("WITH");

            void forEach(SqlStringifier x, SqlCteNode c)
            {
                sb.AppendLineAndIndent();
                c.ToString(x);
            }
            Ctes.ToString(sb, forEach, x => x.AppendLineAndIndent(","));

            Statement.ToString(sb);
        }
    }

    public class SqlCteNode : SqlNode
    {
        public SqlIdentifierNode Name { get; set; }
        public SqlNode Select { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Name.ToString(sb);
            sb.Append(" AS (");
            sb.IncreaseIndent();
            sb.AppendLineAndIndent();
            Select.ToString(sb);
            sb.AppendLineAndIndent();
            sb.DecreaseIndent();
            sb.Append(")");
        }
    }
}
