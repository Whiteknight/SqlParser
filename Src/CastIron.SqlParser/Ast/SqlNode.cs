using System.Text;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public abstract class SqlNode
    {
        public abstract void ToString(StringBuilder sb, int level);

        public override string ToString()
        {
            var sb = new StringBuilder();
            ToString(sb, 0);
            return sb.ToString();
        }

        public Location Location { get; set; }
    }
}