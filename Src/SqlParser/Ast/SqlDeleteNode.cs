using SqlParser.Symbols;

namespace SqlParser.Ast
{
    public class SqlDeleteNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlNode Source { get; set; }
        public SqlNode WhereClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitDelete(this);

        public SqlDeleteNode Update(SqlNode source, SqlNode where)
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