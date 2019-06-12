using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlWithNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlListNode<SqlCteNode> Ctes { get; set; }
        public SqlNode Statement { get; set; }
        public SymbolTable Symbols { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitWith(this);
        public SqlWithNode Update(SqlListNode<SqlCteNode> ctes, SqlNode stmt)
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

    public class SqlCteNode : SqlNode
    {
        public SqlIdentifierNode Name { get; set; }
        public SqlNode Select { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitCte(this);

        public SqlCteNode Update(SqlIdentifierNode name, SqlNode select)
        {
            if (name == Name && select == Select)
                return this;
            return new SqlCteNode
            {
                Location = Location,
                Name = name,
                Select = select
            };
        }
    }
}
