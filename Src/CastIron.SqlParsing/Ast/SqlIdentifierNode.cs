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

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("[");
            sb.Append(Name);
            sb.Append("]");
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

        public override void ToString(SqlStringifier sb)
        {
            if (Qualifier != null)
            {
                Qualifier.ToString(sb);
                sb.Append(".");
            }

            Identifier.ToString(sb);
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

        public override void ToString(SqlStringifier sb)
        {
            if (Server != null)
            {
                Server.ToString(sb);
                sb.Append(".");
            }

            if (Database != null)
            {
                Database.ToString(sb);
                sb.Append(".");
            }

            if (Schema != null)
            {
                Schema.ToString(sb);
                sb.Append(".");
            }

            Name.ToString(sb);
        }
    }
}