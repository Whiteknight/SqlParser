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

    public class SqlObjectIdentifierNode :SqlNode
    {
        public SqlObjectIdentifierNode()
        {
        }

        public SqlObjectIdentifierNode(string database, string schema, string name)
            : this(null, database, schema, name)
        {
        }

        public SqlObjectIdentifierNode(string schema, string name)
            : this(null, null, schema, name)
        {
        }

        public SqlObjectIdentifierNode(string name)
            : this(null, null, null, name)
        {
        }

        public SqlObjectIdentifierNode(string server, string database, string schema, string name)
        {
            if (!string.IsNullOrEmpty(server))
                Server = new SqlIdentifierNode(server);
            if (!string.IsNullOrEmpty(database))
                Database = new SqlIdentifierNode(database);
            if (!string.IsNullOrEmpty(schema))
                Schema = new SqlIdentifierNode(schema);
            Name = new SqlIdentifierNode(name);
        }

        public SqlNode Server { get; set; }
        public SqlNode Database { get; set; }
        public SqlNode Schema { get; set; }
        public SqlNode Name { get; set; }

        public override void ToString(StringBuilder sb, int level)
        {
            if (Server != null)
            {
                Server.ToString(sb, level);
                sb.Append(".");
            }

            if (Database != null)
            {
                Database.ToString(sb, level);
                sb.Append(".");
            }

            if (Schema != null)
            {
                Schema.ToString(sb, level);
                sb.Append(".");
            }

            Name.ToString(sb, level);
        }
    }
}