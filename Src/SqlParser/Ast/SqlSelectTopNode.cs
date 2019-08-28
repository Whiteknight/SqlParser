namespace SqlParser.Ast
{
    public class SqlSelectTopNode : SqlNode
    {
        public SqlNode Value { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitTop(this);

        public SqlSelectTopNode Update(SqlNode value, bool percent, bool withTies)
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
