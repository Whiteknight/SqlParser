using SqlParser.Tokenizing;

namespace SqlParser.Ast
{
    public class SqlNullNode : SqlNode
    {
        public SqlNullNode()
        {
        }

        public SqlNullNode(SqlToken t)
        {
            Location = t.Location;
        }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitNull(this);
    }
}