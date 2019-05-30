using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectOrderByClauseNode : SqlNode
    {
        public SqlListNode<SqlOrderByEntryNode> Entries { get; set; }

        public SqlNode Offset { get; set; }
        public SqlNode Limit { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.AppendLine("ORDER BY");
            sb.AppendIndent(level + 1);
            Entries.Children[0].ToString(sb, level + 1);
            for (int i = 1; i < Entries.Children.Count; i++)
            {
                sb.AppendLine(",");
                sb.AppendIndent(level + 1);
                Entries.Children[i].ToString(sb, level + 1);
            }
            if (Offset != null || Limit != null)
            {
                sb.AppendLine();
                sb.AppendIndent(level + 1);
                if (Offset != null)
                {
                    sb.Append("OFFSET ");
                    Offset.ToString(sb, level + 1);
                    sb.Append(" ROWS ");
                }

                if (Limit != null)
                {
                    sb.Append("FETCH NEXT ");
                    Limit.ToString(sb, level + 1);
                    sb.Append("ROWS ONLY");
                }
            }
        }
    }

    public class SqlOrderByEntryNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public string Direction { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Source.ToString(sb, level);
            sb.Append(" ");
            sb.Append(Direction ?? "ASC");
        }
    }
}