using SqlParser.Tokenizing;

namespace SqlParser.Ast
{
    public class SqlVariableNode : SqlNode
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

        public string Name { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitVariable(this);
    }
}