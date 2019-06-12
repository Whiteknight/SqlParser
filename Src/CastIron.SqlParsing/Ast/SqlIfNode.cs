namespace CastIron.SqlParsing.Ast
{
    public class SqlIfNode : SqlNode
    {
        public SqlNode Condition { get; set; }
        public SqlNode Then { get; set; }
        public SqlNode Else { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitIf(this);

        public SqlIfNode Update(SqlNode cond, SqlNode then, SqlNode e)
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

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("IF (");
            Condition.ToString(sb);
            sb.AppendLine(")");
            sb.IncreaseIndent();
            sb.WriteIndent();
            Then.ToString(sb);
            sb.AppendLine();
            sb.DecreaseIndent();
            if (Else != null)
            {
                sb.AppendLine("ELSE");
                sb.IncreaseIndent();
                sb.WriteIndent();
                Else.ToString(sb);
                sb.AppendLine();
                sb.DecreaseIndent();
            }
        }
    }
}