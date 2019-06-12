using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
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

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitVariable(this);
    }
}