using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlExecuteNode : ISqlNode
    {
        public ISqlNode Name { get; set; }

        public SqlListNode<SqlExecuteArgumentNode> Arguments { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitExecute(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlExecuteNode Update(ISqlNode name, SqlListNode<SqlExecuteArgumentNode> args)
        {
            if (name == Name && args == Arguments)
                return this;
            return new SqlExecuteNode
            {
                Name = name,
                Arguments = args
            };
        }
    }

    public class SqlExecuteArgumentNode : ISqlNode
    {
        public SqlVariableNode AssignVariable { get; set; }
        public ISqlNode Value { get; set; }
        public bool IsOut { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitExecuteArgument(this);

        public override string ToString() => StringifyVisitor.ToString(this);

        public Location Location { get; set; }

        public SqlExecuteArgumentNode Update(SqlVariableNode assign, ISqlNode value, bool isOut)
        {
            if (assign == AssignVariable && Value == value && isOut == IsOut)
                return this;
            return new SqlExecuteArgumentNode
            {
                Location = Location,
                AssignVariable = assign,
                IsOut = isOut,
                Value = value
            };
        }
    }
}
