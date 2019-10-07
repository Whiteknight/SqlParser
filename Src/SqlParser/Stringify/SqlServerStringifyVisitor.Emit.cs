using System;
using System.IO;
using System.Text;
using SqlParser.Ast;

namespace SqlParser.Stringify
{
    public partial class SqlServerStringifyVisitor
    {
        private readonly TextWriter _tw;
        private int _indent;

        public SqlServerStringifyVisitor(TextWriter tw)
        {
            _tw = tw ?? throw new ArgumentNullException(nameof(tw));
            _indent = 0;
        }

        public SqlServerStringifyVisitor(StringBuilder sb)
            : this(new StringWriter(sb))
        {
        }


        public void AppendLineAndIndent(string s = "")
        {
            AppendLine(s);
            WriteIndent();
        }

        public void AppendLine(params object[] args)
        {
            Append(args);
            _tw.WriteLine();
        }

        public void Append(params object[] args)
        {
            foreach (var arg in args)
            {
                if (arg == null)
                    continue;
                if (arg is string s)
                    _tw.Write(s);
                else if (arg is SqlNode n)
                    Visit(n);
            }
        }

        public void WriteIndent()
        {
            if (_indent <= 0)
                return;
            _tw.Write(new string(' ', _indent * 4));
        }
        public void IncreaseIndent() => _indent++;
        public void DecreaseIndent() => _indent--;
    }
}
