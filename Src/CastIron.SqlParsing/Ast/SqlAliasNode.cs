﻿namespace CastIron.SqlParsing.Ast
{
    public class SqlAliasNode  : SqlNode
    {
        public SqlNode Source { get; set; }
        public SqlIdentifierNode Alias { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitAlias(this);

        public SqlAliasNode Update(SqlNode source, SqlIdentifierNode alias)
        {
            if (source == Source && alias == Alias)
                return this;
            return new SqlAliasNode
            {
                Location = Location,
                Source = source,
                Alias = alias
            };
        }
    }
}