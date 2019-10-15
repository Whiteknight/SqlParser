using System.Collections.Generic;
using System.Text;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlUnknownStatementNode : ISqlNode
    {
        public List<SqlToken> Tokens { get; set; }
        public Location Location { get; set; }
        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitUnknown(this);

        public override string ToString()
        {
            if (Tokens == null || Tokens.Count == 0)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append(Tokens[0].Value);

            for (int i = 1; i < Tokens.Count; i++)
            {
                sb.Append(" ");
                sb.Append(Tokens[i].Value);
            }

            return sb.ToString();
        }
    }
}
