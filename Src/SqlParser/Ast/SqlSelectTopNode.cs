using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlSelectTopNode : ISqlNode
    {
        public ISqlNode Value { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitTop(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlSelectTopNode Update(ISqlNode value, bool percent, bool withTies)
        {
            if (value == Value && percent == Percent && withTies == WithTies)
                return this;
            return new SqlSelectTopNode
            {
                Location = Location,
                Value = value,
                Percent = percent,
                WithTies = withTies
            };
        }
    }
}
