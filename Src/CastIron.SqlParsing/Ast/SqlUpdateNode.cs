using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlUpdateNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlNode Source { get; set; }
        public SqlListNode<SqlInfixOperationNode> SetClause { get; set; }
        public SqlNode WhereClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitUpdate(this);

        public SqlUpdateNode Update(SqlNode source, SqlListNode<SqlInfixOperationNode> set, SqlNode where)
        {
            if (source == Source && set == SetClause && where == WhereClause)
                return this;
            return new SqlUpdateNode
            {
                Location = Location,
                Source = source,
                SetClause = set,
                WhereClause = where
            };
        }
    }
}