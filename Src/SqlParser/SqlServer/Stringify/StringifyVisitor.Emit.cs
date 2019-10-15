using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SqlParser.Ast;
using SqlParser.Symbols;

namespace SqlParser.SqlServer.Stringify
{
    public partial class StringifyVisitor
    {
        private readonly TextWriter _tw;
        private int _indent;

        public StringifyVisitor(TextWriter tw)
        {
            _tw = tw ?? throw new ArgumentNullException(nameof(tw));
            _indent = 0;
            _allSymbolTables = new Stack<SymbolTable>();
            _nonNullSymbolTables = new Stack<SymbolTable>();
        }

        public StringifyVisitor(StringBuilder sb)
            : this(new StringWriter(sb))
        {
        }

        public static string ToString(ISqlNode node)
        {
            var sb = new StringBuilder();
            var visitor = new StringifyVisitor(sb);
            visitor.Visit(node);
            return sb.ToString();
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
                else if (arg is ISqlNode n)
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
