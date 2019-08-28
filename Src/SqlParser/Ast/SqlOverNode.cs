namespace SqlParser.Ast
{
    public class SqlOverNode : SqlNode
    {
        public SqlNode Expression { get; set; }
        public SqlNode PartitionBy { get; set; }
        public SqlNode OrderBy { get; set; }
        public SqlNode RowsRange { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitOver(this);

        public SqlOverNode Update(SqlNode expr, SqlNode part, SqlNode orderBy, SqlNode rows)
        {
            if (expr == Expression && part == PartitionBy && orderBy == OrderBy && rows == RowsRange)
                return this;
            return new SqlOverNode
            {
                Expression = expr,
                Location = Location,
                OrderBy = orderBy,
                PartitionBy = part,
                RowsRange = rows
            };
        }
    }
}
