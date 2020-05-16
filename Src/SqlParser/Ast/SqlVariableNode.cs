using ParserObjects;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlVariableNode : SqlNode, ISqlNode
    {
        public SqlVariableNode()
        {
        }

        public SqlVariableNode(SqlToken token)
        {
            Name = token.Value;
            Location = token.Location;
        }

        public SqlVariableNode(string name, Location l = null)
        {
            Name = name;
            Location = l;
        }

        public Location Location { get; set; }

        public string Name { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitVariable(this);

        public string GetBareName() => Name.StartsWith("@") ? Name.Substring(1) : Name;

        public override string ToString() => Name;
    }
}