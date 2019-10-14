using SqlParser.SqlServer.Stringify;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlMergeNode : ISqlNode
    {
        // TODO: Symbol table? source and target can be aliased, we also want to insert 'TARGET' and 'SOURCE' as symbols
        public ISqlNode Target { get; set; }
        public ISqlNode Source { get; set; }
        public ISqlNode MergeCondition { get; set; }
        public ISqlNode Matched { get; set; }
        public SqlMergeInsertNode NotMatchedByTarget { get; set; }
        public ISqlNode NotMatchedBySource { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitMerge(this);

        

        public Location Location { get; set; }

        public SqlMergeNode Update(ISqlNode target, ISqlNode source, ISqlNode condition, ISqlNode matched, SqlMergeInsertNode nmatchtarget, ISqlNode nmatchsource)
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

    public class SqlMergeUpdateNode : ISqlNode
    {
        public SqlListNode<SqlInfixOperationNode> SetClause { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitMergeUpdate(this);

        

        public Location Location { get; set; }

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

    public class SqlMergeInsertNode : ISqlNode
    {
        public SqlListNode<SqlIdentifierNode> Columns { get; set; }
        public ISqlNode Source { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitMergeInsert(this);

        

        public Location Location { get; set; }

        public SqlMergeInsertNode Update(SqlListNode<SqlIdentifierNode> columns, ISqlNode source)
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
