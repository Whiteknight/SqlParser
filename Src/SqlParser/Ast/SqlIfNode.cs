using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlIfNode : ISqlNode
    {
        public ISqlNode Condition { get; set; }
        public ISqlNode Then { get; set; }
        public ISqlNode Else { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitIf(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlIfNode Update(ISqlNode cond, ISqlNode then, ISqlNode e)
        {
            if (cond == Condition && Then == then && e == Else)
                return this;
            return new SqlIfNode
            {
                Condition = cond,
                Then = then,
                Else = e
            };
        }
    }
}