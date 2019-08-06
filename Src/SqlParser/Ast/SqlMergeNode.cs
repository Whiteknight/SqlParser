namespace SqlParser.Ast
{
    public class SqlMergeNode : SqlNode
    {
        // TODO: Symbol table? source and target can be aliased, we also want to insert 'TARGET' and 'SOURCE' as symbols
        public SqlNode Target { get; set; }
        public SqlNode Source { get; set; }
        public SqlNode MergeCondition { get; set; }
        public SqlNode Matched { get; set; }
        public SqlNode NotMatchedByTarget { get; set; }
        public SqlNode NotMatchedBySource { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitMerge(this);

        public SqlMergeNode Update(SqlNode target, SqlNode source, SqlNode condition, SqlNode matched, SqlNode nmatchtarget, SqlNode nmatchsource)
        {
            if (Target == target && Source == source && MergeCondition == condition && Matched == matched && NotMatchedByTarget == nmatchtarget && NotMatchedBySource == nmatchsource)
                return this;
            return new SqlMergeNode
            {
                Location = Location,
                Target = target,
                Source = source,
                MergeCondition = condition,
                Matched = matched,
                NotMatchedBySource = nmatchsource,
                NotMatchedByTarget = nmatchtarget
            };
        }
    }

    public class SqlMergeUpdateNode : SqlNode
    {
        public SqlListNode<SqlInfixOperationNode> SetClause { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitMergeUpdate(this);

        public SqlMergeUpdateNode Update(SqlListNode<SqlInfixOperationNode> set)
        {
            if (set == SetClause)
                return this;
            return new SqlMergeUpdateNode
            {
                Location = Location,
                SetClause = set,
            };
        }
    }

    public class SqlMergeInsertNode : SqlNode
    {
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public SqlNode Source { get; set; }

        public override SqlNode Accept(SqlNodeVisitor visitor) => visitor.VisitMergeInsert(this);

        public SqlMergeInsertNode Update(SqlListNode<SqlIdentifierNode> columns, SqlNode source)
        {
            if (columns == Columns && source == Source)
                return this;
            return new SqlMergeInsertNode
            {
                Location = Location,
                Columns = columns,
                Source = source
            };
        }
    }
}
