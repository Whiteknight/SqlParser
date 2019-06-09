namespace CastIron.SqlParsing.Ast
{
    public class SqlWhereNode : SqlNode
    {
        public SqlNode SearchCondition { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.AppendLine("WHERE");
            sb.IncreaseIndent();
            sb.WriteIndent();
            SearchCondition.ToString(sb);
            sb.DecreaseIndent();
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitWhere(this);

        public SqlWhereNode Update(SqlNode searchCondition)
        {
            return searchCondition == SearchCondition
                ? this
                : new SqlWhereNode
                {
                    Location = Location,
                    SearchCondition = SearchCondition
                };
        }
    }
}