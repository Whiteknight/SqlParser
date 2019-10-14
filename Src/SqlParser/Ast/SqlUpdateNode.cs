using SqlParser.SqlServer.Stringify;
using SqlParser.SqlServer.Symbols;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlUpdateNode : ISqlNode, ISqlSymbolScopeNode
    {
        public ISqlNode Source { get; set; }
        public SqlListNode<SqlInfixOperationNode> SetClause { get; set; }
        public ISqlNode WhereClause { get; set; }
        public SymbolTable Symbols { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitUpdate(this);

        

        public Location Location { get; set; }

        public SqlUpdateNode Update(ISqlNode source, SqlListNode<SqlInfixOperationNode> set, ISqlNode where)
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