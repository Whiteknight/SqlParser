using SqlParser.SqlServer.Stringify;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlStringNode : ISqlNode
    {
        public SqlStringNode()
        {
        }

        public SqlStringNode(SqlToken token)
        {
            Value = token.Value;
            Location = token.Location;
        }

        public SqlStringNode(string value)
        {
            Value = value;
        }

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public string Value { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitString(this);
    }
}
