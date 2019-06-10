namespace CastIron.SqlParsing.Ast
{
    public class SqlSelectOrderByClauseNode : SqlNode
    {
        public SqlListNode<SqlOrderByEntryNode> Entries { get; set; }

        public SqlNode Offset { get; set; }
        public SqlNode Limit { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("ORDER BY");
            sb.IncreaseIndent();
            Entries.ToString(sb, (x, c) =>
            {
                sb.AppendLineAndIndent();
                c.ToString(sb);
            }, x => x.Append(","));
            if (Offset != null || Limit != null)
            {
                sb.AppendLineAndIndent();
                if (Offset != null)
                {
                    sb.Append("OFFSET ");
                    Offset.ToString(sb);
                    sb.Append(" ROWS ");
                }

                if (Limit != null)
                {
                    sb.Append("FETCH NEXT ");
                    Limit.ToString(sb);
                    sb.Append(" ROWS ONLY");
                }
            }
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitOrderBy(this);

        public SqlSelectOrderByClauseNode Update(SqlListNode<SqlOrderByEntryNode> entries, SqlNode offset, SqlNode limit)
        {
            if (entries == Entries && offset == Offset && limit == Limit)
                return this;
            return new SqlSelectOrderByClauseNode
            {
                Location = Location,
                Entries = entries,
                Offset = offset,
                Limit = limit
            };
        }
    }

    public class SqlOrderByEntryNode : SqlNode
    {
        public SqlNode Source { get; set; }
        public string Direction { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            Source.ToString(sb);
            if (!string.IsNullOrEmpty(Direction))
            {
                sb.Append(" ");
                sb.Append(Direction);
            }
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitOrderByEntry(this);

        public SqlOrderByEntryNode Update(SqlNode source, string direction)
        {
            if (source == Source && direction == Direction)
                return this;
            return new SqlOrderByEntryNode
            {
                Location = Location,
                Source = source,
                Direction = direction
            };
        }
    }
}