using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlBetweenOperationNode : ISqlNode
    {
        public bool Not { get; set; }
        public ISqlNode Left { get; set; }
        public ISqlNode Low { get; set; }
        public ISqlNode High { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitBetween(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlBetweenOperationNode Update(bool not, ISqlNode left, ISqlNode low, ISqlNode high)
        {
            if (not == Not && left == Left && low == Low && high == High)
                return this;
            return new SqlBetweenOperationNode
            {
                Location = Location,
                Left = left,
                Low = low,
                High = high
            };
        }
    }
}