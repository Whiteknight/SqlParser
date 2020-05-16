using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlBetweenOperationNode : SqlNode, ISqlNode
    {
        public bool Not { get; set; }
        public ISqlNode Left { get; set; }
        public ISqlNode Low { get; set; }
        public ISqlNode High { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitBetween(this);

        

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