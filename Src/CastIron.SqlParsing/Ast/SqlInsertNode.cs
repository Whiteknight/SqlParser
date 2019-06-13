﻿using CastIron.SqlParsing.Symbols;

namespace CastIron.SqlParsing.Ast
{
    public class SqlInsertNode : SqlNode, ISqlSymbolScopeNode
    {
        public SqlNode Table { get; set; }
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public SqlNode Source { get; set; }
        public SymbolTable Symbols { get; set; }

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
}