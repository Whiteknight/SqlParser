using System.Text;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public class SqlIdentifierNode : SqlNode
    {
        public SqlIdentifierNode()
        {
        }

        public SqlIdentifierNode(SqlToken token)
        {
            Name = token.Value;
            Location = token.Location;
        }

        public SqlIdentifierNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append(Name);
        }
    }

    public class SqlQualifiedIdentifierNode : SqlNode
    {
        public SqlQualifiedIdentifierNode()
        {
        }

        public SqlQualifiedIdentifierNode(SqlToken token)
        {
            Location = token.Location;
            Identifier = new SqlIdentifierNode(token);
        }

        public SqlIdentifierNode Qualifier { get; set; }
        public SqlNode Identifier { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            if (Qualifier != null)
            {
                Qualifier.ToString(sb, level);
                sb.Append(".");
            }

            Identifier.ToString(sb, level);
        }
    }
}