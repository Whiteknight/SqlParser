using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlInsertNode : SqlNode
    {
        public SqlNode Table { get; set; }
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public SqlNode Source { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.Append("INSERT INTO ");
            Table.ToString(sb, level);
            sb.Append("(");
            Columns.ToString(sb, level);
            sb.AppendLine(")");
            sb.AppendIndent(level + 1);
            Source.ToString(sb, level + 1);
        }
    }

    public class SqlInsertValuesNode : SqlNode
    {
        public SqlNode Values { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append("VALUES ");
            Values.ToString(sb, level);
        }
    }

    public class SqlDefaultValuesNode : SqlNode
    {
        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append("DEFAULT VALUES");
        }
    }

    public class SqlUpdateNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlListNode<SqlNode> SetClause { get; set; }
        public SqlWhereNode WhereClause { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.Append("UPDATE ");
            Source.ToString(sb, level);
            sb.Append(" SET ");
            SetClause.ToString(sb, level);
            WhereClause?.ToString(sb, level);
        }
    }
}