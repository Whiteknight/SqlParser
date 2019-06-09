namespace CastIron.SqlParsing.Ast
{
    public class SqlInNode : SqlNode
    {
        public bool Not { get; set; }
        public SqlNode Search { get; set; }
        public SqlListNode<SqlNode> Items { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Search.ToString(sb);
            if (Not)
                sb.Append(" NOT");
            sb.Append(" IN (");
            Items.ToString(sb);
            sb.Append(")");
        }

        public SqlInNode Update(bool not, SqlNode search, SqlListNode<SqlNode> items)
        {
            if (not == Not && search == Search && items == Items)
                return this;
            return new SqlInNode
            {
                Location = Location,
                Not = not,
                Items = items,
                Search = search
            };
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitIn(this);
    }
}
