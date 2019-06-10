namespace CastIron.SqlParsing.Ast
{
    public class SqlSetNode : SqlNode
    {
        public SqlVariableNode Variable { get; set; }
        public SqlNode Right { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitSet(this);

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("SET ");
            Variable.ToString(sb);
            sb.Append(" = ");
            Right.ToString(sb);
        }

        public SqlSetNode Update(SqlVariableNode v, SqlNode right)
        {
            if (v == Variable && right == Right)
                return this;
            return new SqlSetNode
            {
                Location = Location,
                Right = right,
                Variable = v
            };
        }
    }
}