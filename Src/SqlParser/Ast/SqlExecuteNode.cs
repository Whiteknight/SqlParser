namespace SqlParser.Ast
{
    public class SqlExecuteNode : SqlNode
    {
        public SqlNode Name { get; set; }

        public SqlListNode<SqlExecuteArgumentNode> Arguments { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitExecute(this);

        public SqlExecuteNode Update(SqlNode name, SqlListNode<SqlExecuteArgumentNode> args)
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

    public class SqlExecuteArgumentNode : SqlNode
    {
        public SqlVariableNode AssignVariable { get; set; }
        public SqlNode Value { get; set; }
        public bool IsOut { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitExecuteArgument(this);

        public SqlExecuteArgumentNode Update(SqlVariableNode assign, SqlNode value, bool isOut)
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
