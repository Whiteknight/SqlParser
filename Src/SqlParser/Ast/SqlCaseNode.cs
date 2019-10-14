using System.Collections.Generic;
using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlCaseNode : ISqlNode
    {
        public SqlCaseNode()
        {
            WhenExpressions = new List<SqlCaseWhenNode>();
        }

        public ISqlNode InputExpression { get; set; }
        public List<SqlCaseWhenNode> WhenExpressions { get; set; }
        public ISqlNode ElseExpression { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitCase(this);

        

        public Location Location { get; set; }

        public SqlCaseNode Update(ISqlNode input, List<SqlCaseWhenNode> whens, ISqlNode e)
        {
            if (input == InputExpression && whens == WhenExpressions && e == ElseExpression)
                return this;
            return new SqlCaseNode
            {
                Location = Location,
                InputExpression = input,
                WhenExpressions = whens,
                ElseExpression = e
            };
        }
    }

    public class SqlCaseWhenNode: ISqlNode
    {
        public ISqlNode Condition { get; set; }
        public ISqlNode Result { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitCaseWhen(this);

        

        public Location Location { get; set; }

        public SqlCaseWhenNode Update(ISqlNode cond, ISqlNode result)
        {
            if (cond == Condition && result == Result)
                return this;
            return new SqlCaseWhenNode
            {
                Location = Location,
                Condition = cond,
                Result = result
            };
        }
    }
}
