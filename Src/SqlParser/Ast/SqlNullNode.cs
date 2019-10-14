using SqlParser.SqlServer.Stringify;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlNullNode : ISqlNode
    {
        public SqlNullNode()
        {
        }

        public SqlNullNode(SqlToken t)
        {
            Location = t.Location;
        }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitNull(this);

        

        public Location Location { get; set; }
    }
}