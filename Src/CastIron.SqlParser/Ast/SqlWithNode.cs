using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlWithNode : SqlNode
    {
        public SqlListNode<SqlCteNode> Ctes { get; set; }
        public SqlNode Statement { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.AppendLine("WITH");
            Ctes.ToString(sb, level);

            Statement.ToString(sb, level);
        }
    }

    public class SqlCteNode : SqlNode
    {
        public SqlIdentifierNode Name { get; set; }
        public SqlNode Select { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            Name.ToString(sb, level);
            sb.AppendLine(" AS (");
            Select.ToString(sb, level + 1);
            sb.AppendLine();
            sb.AppendIndent(level);
            sb.AppendLine(")");
        }
    }
}
