using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlTopLimitNode : SqlNode, ISqlNode
    {
        public ISqlNode Value { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitTopLimit(this);

        public Location Location { get; set; }

        public SqlTopLimitNode Update(ISqlNode value, bool percent, bool withTies)
        {
            if (value == Value && percent == Percent && withTies == WithTies)
                return this;
            return new SqlTopLimitNode
            {
                Location = Location,
                Value = value,
                Percent = percent,
                WithTies = withTies
            };
        }
    }
}
