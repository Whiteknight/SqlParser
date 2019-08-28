using SqlParser.Tokenizing;

namespace SqlParser.Ast
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

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitIdentifier(this);
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

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitQualifiedIdentifier(this);

        public SqlQualifiedIdentifierNode Update(SqlIdentifierNode qualfier, SqlNode id)
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

        public SqlIdentifierNode Server { get; set; }
        public SqlIdentifierNode Database { get; set; }
        public SqlIdentifierNode Schema { get; set; }
        public SqlIdentifierNode Name { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitObjectIdentifier(this);

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
    }
}