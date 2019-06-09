namespace CastIron.SqlParsing.Ast
{
    public class SqlDeleteNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlWhereNode WhereClause { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("DELETE FROM ");
            Source.ToString(sb);
            sb.Append(" ");
            WhereClause?.ToString(sb);
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitDelete(this);

        public SqlDeleteNode Update(SqlNode source, SqlWhereNode where)
        {
            if (source == Source && where == WhereClause)
                return this;
            return new SqlDeleteNode
            {
                Location = Location,
                Source = source,
                WhereClause = where
            };
        }
    }
}