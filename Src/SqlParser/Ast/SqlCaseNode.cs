using System.Collections.Generic;

namespace SqlParser.Ast
{
    public class SqlCaseNode : SqlNode
    {
        public SqlCaseNode()
        {
            WhenExpressions = new List<SqlCaseWhenNode>();
        }

        public SqlNode InputExpression { get; set; }
        public List<SqlCaseWhenNode> WhenExpressions { get; set; }
        public SqlNode ElseExpression { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitCase(this);

        public SqlCaseNode Update(SqlNode input, List<SqlCaseWhenNode> whens, SqlNode e)
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

    public class SqlCaseWhenNode: SqlNode
    {
        public SqlNode Condition { get; set; }
        public SqlNode Result { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitCaseWhen(this);

        public SqlCaseWhenNode Update(SqlNode cond, SqlNode result)
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
