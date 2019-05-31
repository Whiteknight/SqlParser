using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlDeleteNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlWhereNode WhereClause { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.AppendIndent(level);
            sb.Append("DELETE FROM ");
            Source.ToString(sb, level);
            sb.Append(" ");
            WhereClause.ToString(sb, level);
        }
    }
}