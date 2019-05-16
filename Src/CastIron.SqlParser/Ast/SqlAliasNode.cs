using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlAliasNode  : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlIdentifierNode Alias { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            Source.ToString(sb, level);
            sb.Append(" AS ");
            Alias.ToString(sb, level);
        }
    }
}