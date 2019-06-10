using System.Text;

namespace CastIron.SqlParsing.Ast
{
    public class SqlStringifier 
    {
        private readonly StringBuilder _sb;
        private int _indent;

        public SqlStringifier(StringBuilder sb)
        {
            _sb = sb;
            _indent = 0;
        }

        public void AppendLineAndIndent(string s = "")
        {
            AppendLine(s);
            WriteIndent();
        }

        public void AppendLine(string s = "") => _sb.AppendLine(s);
        public void Append(string s) => _sb.Append(s);
        public void WriteIndent()
        {
            if (_indent <= 0)
                return;
            _sb.Append(new string(' ', _indent * 4));
        }
        public void IncreaseIndent() => _indent++;
        public void DecreaseIndent() => _indent--;
    }
}
