using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlInNode : SqlNode, ISqlNode
    {
        public bool Not { get; set; }
        public ISqlNode Search { get; set; }
        public SqlListNode<ISqlNode> Items { get; set; }

        

        public Location Location { get; set; }

        public SqlInNode Update(bool not, ISqlNode search, SqlListNode<ISqlNode> items)
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

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitIn(this);
    }
}
