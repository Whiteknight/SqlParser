using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlFunctionCallNode : ISqlNode
    {
        public ISqlNode Name { get; set; }
        public SqlListNode<ISqlNode> Arguments { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitFunctionCall(this);

        

        public Location Location { get; set; }

        public SqlFunctionCallNode Update(ISqlNode name, SqlListNode<ISqlNode> args)
        {
            if (name == Name && args == Arguments)
                return this;
            return new SqlFunctionCallNode
            {
                Location = Location,
                Name = name,
                Arguments = args
            };
        }
    }
}
