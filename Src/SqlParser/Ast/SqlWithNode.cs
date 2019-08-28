using SqlParser.Symbols;

namespace SqlParser.Ast
{
    public class SqlWithNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlListNode<SqlWithCteNode> Ctes { get; set; }
        public SqlNode Statement { get; set; }
        public SymbolTable Symbols { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitWith(this);
        public SqlWithNode Update(SqlListNode<SqlWithCteNode> ctes, SqlNode stmt)
        {
            if (ctes == Ctes && Statement == stmt)
                return this;
            return new SqlWithNode
            {
                Location = Location,
                Ctes = ctes,
                Statement = stmt
            };
        }
    }

    public class SqlWithCteNode : SqlNode
    {
        public SqlIdentifierNode Name { get; set; }
        public SqlNode Select { get; set; }
        public SqlListNode<SqlIdentifierNode> ColumnNames { get; set; }
        public bool Recursive { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitWithCte(this);

        public SqlWithCteNode Update(SqlIdentifierNode name, SqlListNode<SqlIdentifierNode>  columns, SqlNode select, bool recursive)
        {
            if (name == Name && columns == ColumnNames && select == Select && recursive == Recursive)
                return this;
            return new SqlWithCteNode
            {
                Location = Location,
                Name = name,
                ColumnNames = columns,
                Select = select,
                Recursive = recursive
            };
        }
    }
}
