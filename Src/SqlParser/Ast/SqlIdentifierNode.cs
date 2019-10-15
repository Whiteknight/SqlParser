using System.Text;
using SqlParser.SqlServer.Stringify;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlIdentifierNode : ISqlNode
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

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitIdentifier(this);

        

        public Location Location { get; set; }
    }

    public class SqlQualifiedIdentifierNode : ISqlNode
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
        public ISqlNode Identifier { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitQualifiedIdentifier(this);

        

        public Location Location { get; set; }

        public SqlQualifiedIdentifierNode Update(SqlIdentifierNode qualfier, ISqlNode id)
        {
            if (qualfier == Qualifier && id == Identifier)
                return this;
            return new SqlQualifiedIdentifierNode
            {
                Location = Location,
                Identifier = id,
                Qualifier = qualfier
            };
        }
    }

    public class SqlObjectIdentifierNode : ISqlNode
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

        public SqlIdentifierNode Server { get; set; }
        public SqlIdentifierNode Database { get; set; }
        public SqlIdentifierNode Schema { get; set; }
        public SqlIdentifierNode Name { get; set; }

        public Location Location { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitObjectIdentifier(this);

        public SqlObjectIdentifierNode Update(SqlIdentifierNode server, SqlIdentifierNode db, SqlIdentifierNode schema, SqlIdentifierNode name)
        {
            if (server == Server && db == Database && schema == Schema && name == Name)
                return this;
            return new SqlObjectIdentifierNode
            {
                Location = Location,
                Server = server,
                Database = db,
                Schema = schema,
                Name = name
            };
        }

        // Get a simple string representation of this node, though StringifyVisitor.Visit(this) does a 
        // correct job for a dialect
        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Server != null)
            {
                sb.Append(Server.Name);
                sb.Append(".");
            }
            if (Database != null)
            {
                sb.Append(Database.Name);
                sb.Append(".");
            }
            if (Schema != null)
            {
                sb.Append(Schema.Name);
                sb.Append(".");
            }

            sb.Append(Name.Name);
            return sb.ToString();
        }
    }
}