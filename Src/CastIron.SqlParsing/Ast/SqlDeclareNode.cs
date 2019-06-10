namespace CastIron.SqlParsing.Ast
{ 
    public class SqlDeclareNode : SqlNode
    {
        public SqlVariableNode Variable { get; set; }

        public SqlNode DataType { get; set; }
        public SqlNode Initializer { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitDeclare(this);


        public override void ToString(SqlStringifier sb)
        {
            sb.Append("DECLARE ");
            Variable.ToString(sb);
            sb.Append(" ");
            DataType.ToString(sb);
            if (Initializer != null)
            {
                sb.Append(" = ");
                Initializer.ToString(sb);
            }
        }

        public SqlDeclareNode Update(SqlVariableNode v, SqlNode dataType, SqlNode init)
        {
            if (v == Variable && dataType == DataType && init == Initializer)
                return this;
            return new SqlDeclareNode
            {
                Location = Location,
                DataType = dataType,
                Initializer = init,
                Variable = v
            };
        }
    }
}
