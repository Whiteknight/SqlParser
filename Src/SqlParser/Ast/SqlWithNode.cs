using System.Linq;
using SqlParser.SqlServer.Stringify;
using SqlParser.SqlServer.Symbols;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlWithNode : ISqlNode, ISqlSymbolScopeNode
    {
        

        public Location Location { get; set; }
        public SqlListNode<SqlWithCteNode> Ctes { get; set; }
        public ISqlNode Statement { get; set; }
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitWith(this);
        public SqlWithNode Update(SqlListNode<SqlWithCteNode> ctes, ISqlNode stmt)
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

    public class SqlWithCteNode : ISqlNode
    {
        public Location Location { get; set; }
        public SqlIdentifierNode Name { get; set; }
        public ISqlNode Select { get; set; }
        public SqlListNode<SqlIdentifierNode> ColumnNames { get; set; }
        public bool Recursive { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitWithCte(this);

        public SqlWithCteNode Update(SqlIdentifierNode name, SqlListNode<SqlIdentifierNode>  columns, ISqlNode select, bool recursive)
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

        public void DetectRecursion()
        {
            Recursive = Select.FindOfType<SqlIdentifierNode>().Any(n => n.Name == Name.Name);
        }
    }
}
