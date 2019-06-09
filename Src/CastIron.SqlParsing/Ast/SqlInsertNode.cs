namespace CastIron.SqlParsing.Ast
{
    public class SqlInsertNode : SqlNode
    {
        public SqlNode Table { get; set; }
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public SqlNode Source { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("INSERT INTO ");
            Table.ToString(sb);
            sb.Append("(");
            Columns.ToString(sb, (x, c) => c.ToString(x), x => x.Append(", "));
            sb.AppendLine(")");
            sb.IncreaseIndent();
            sb.WriteIndent();
            Source.ToString(sb);
            sb.DecreaseIndent();
        }
    }

    public class SqlInsertValuesNode : SqlNode
    {
        public SqlListNode<SqlListNode<SqlNode>> Values { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            void between(SqlStringifier x)
            {
                sb.Append(", ");
            }
            void forEach(SqlStringifier x, SqlListNode<SqlNode> child)
            {
                sb.Append("(");
                child.ToString(sb, (y, c) => c.ToString(y), y => y.Append(", "));
                sb.Append(")");
            }

            sb.Append("VALUES ");
            Values.ToString(sb, forEach, between);
        }
    }
}