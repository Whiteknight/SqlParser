using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlOverNode : ISqlNode
    {
        public ISqlNode Expression { get; set; }
        public ISqlNode PartitionBy { get; set; }
        public ISqlNode OrderBy { get; set; }
        public ISqlNode RowsRange { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitOver(this);

        

        public Location Location { get; set; }

        public SqlOverNode Update(ISqlNode expr, ISqlNode part, ISqlNode orderBy, ISqlNode rows)
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
