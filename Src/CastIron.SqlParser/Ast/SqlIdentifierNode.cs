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

        public string Name { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            sb.Append(Name);
        }
    }

    public class SqlColumnIdentifierNode : SqlNode
    {
        public SqlIdentifierNode Table { get; set; }
        public SqlNode Column { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            if (Table != null)
            {
                Table.ToString(sb, level);
                sb.Append(".");
            }

            Column.ToString(sb, level);
        }
    }

    public class SqlTableIdentifierNode : SqlIdentifierNode
    {
        public SqlTableIdentifierNode()
        {
        }

        public SqlTableIdentifierNode(SqlToken token) : base(token)
        {
        }

        public SqlIdentifierNode Schema { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            if (Schema != null)
            {
                Schema.ToString(sb, level);
                sb.Append(".");
            }

            sb.Append(Name);
        }
    }
}