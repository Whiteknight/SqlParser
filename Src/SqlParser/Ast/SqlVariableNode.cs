using SqlParser.SqlServer.Stringify;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlVariableNode : ISqlNode
    {
        public SqlVariableNode()
        {
        }

        public SqlVariableNode(SqlToken token)
        {
            Name = token.Value;
            Location = token.Location;
        }

        public SqlVariableNode(string name)
        {
            Name = name;
        }

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public string Name { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitVariable(this);
    }
}