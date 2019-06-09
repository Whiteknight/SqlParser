using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public static class StringBuilderExtensions
    {
        public static void AppendIndent(this StringBuilder sb, int level)
        {
            if (level <= 0)
                return;
            sb.Append(new string(' ', level * 4));
        }
    }
}