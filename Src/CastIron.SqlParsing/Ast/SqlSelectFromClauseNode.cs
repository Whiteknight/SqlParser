namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectFromClauseNode : SqlNode
    {
        public SqlNode Source { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.AppendLine("FROM ");
            sb.IncreaseIndent();
            sb.WriteIndent();
            Source.ToString(sb);
            sb.DecreaseIndent();
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitFrom(this);

        public SqlSelectFromClauseNode Update(SqlNode source)
        {
            return source == Source
                ? this
                : new SqlSelectFromClauseNode
                {
                    Location = Location,
                    Source = source
                };
        }
    }
}