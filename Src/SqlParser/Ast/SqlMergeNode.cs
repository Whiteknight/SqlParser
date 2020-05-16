using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlMergeNode : SqlNode, ISqlNode
    {
        // TODO: Symbol table? source and target can be aliased, we also want to insert 'TARGET' and 'SOURCE' as symbols
        public ISqlNode Target { get; set; }
        public ISqlNode Source { get; set; }
        public ISqlNode MergeCondition { get; set; }
        public SqlListNode<SqlMergeMatchClauseNode> MatchClauses { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitMerge(this);

        public Location Location { get; set; }

        public SqlMergeNode Update(ISqlNode target, ISqlNode source, ISqlNode condition, SqlListNode<SqlMergeMatchClauseNode> matchClauses)
        {
            if (Target == target && Source == source && MergeCondition == condition && MatchClauses == matchClauses)
                return this;
            return new SqlMergeNode
            {
                Location = Location,
                Target = target,
                Source = source,
                MergeCondition = condition,
                MatchClauses = matchClauses
            };
        }
    }

    public class SqlMergeMatchClauseNode : SqlNode, ISqlNode
    {
        public SqlKeywordNode Keyword { get; set; }
        public ISqlNode Condition { get; set; }
        public ISqlNode Action { get; set; }
        public Location Location { get; set; }

        // TODO: This
        public ISqlNode Accept(INodeVisitorTyped visitor) => this;
        // TODO: Update
    }
}
