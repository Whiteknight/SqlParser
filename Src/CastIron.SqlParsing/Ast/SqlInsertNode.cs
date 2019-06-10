using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlInsertNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlNode Table { get; set; }
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public SqlNode Source { get; set; }
        public SymbolTable Symbols { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            sb.Append("INSERT INTO ");
            Table.ToString(sb);
            sb.Append("(");
            Columns.ToString(sb, (x, c) => c.ToString(x), x => x.Append(", "));
            sb.AppendLine(")");
            sb.IncreaseIndent();
            sb.WriteIndent();
            Source.ToString(sb);
            sb.DecreaseIndent();
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitInsert(this);

        public SqlInsertNode Update(SqlNode table, SqlListNode<SqlIdentifierNode> columns, SqlNode source)
        {
            if (table == Table && columns == Columns && source == Source)
                return this;
            return new SqlInsertNode
            {
                Location = Location,
                Table = table,
                Columns = columns,
                Source = source
            };
        }
    }

    public class SqlInsertValuesNode : SqlNode
    {
        public SqlListNode<SqlListNode<SqlNode>> Values { get; set; }

        public override void ToString(SqlStringifier sb)
        {
            void between(SqlStringifier x)
            {
                sb.Append(", ");
            }
            void forEach(SqlStringifier x, SqlListNode<SqlNode> child)
            {
                sb.Append("(");
                child.ToString(sb, (y, c) => c.ToString(y), y => y.Append(", "));
                sb.Append(")");
            }

            sb.Append("VALUES ");
            Values.ToString(sb, forEach, between);
        }

        public SqlInsertValuesNode Update(SqlListNode<SqlListNode<SqlNode>> values)
        {
            if (values == Values)
                return this;
            return new SqlInsertValuesNode
            {
                Location = Location,
                Values = values
            };
        }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitInsertValues(this);
    }
}