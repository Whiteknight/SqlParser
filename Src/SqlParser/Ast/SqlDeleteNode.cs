using ParserObjects;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlDeleteNode : SqlNode, ISqlNode, ISqlSymbolScopeNode
    {
        public ISqlNode Source { get; set; }
        public ISqlNode WhereClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitDelete(this);

        

        public Location Location { get; set; }

        public SqlDeleteNode Update(ISqlNode source, ISqlNode where)
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