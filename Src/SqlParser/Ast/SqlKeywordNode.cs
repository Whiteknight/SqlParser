using SqlParser.Tokenizing;

namespace SqlParser.Ast
{
    public class SqlKeywordNode : SqlNode
    {
        public SqlKeywordNode()
        {
        }

        public SqlKeywordNode(SqlToken t)
        {
            Keyword = t.Value;
            Location = t.Location;
        }

        public SqlKeywordNode(string keyword)
        {
            Keyword = keyword;
        }

        public string Keyword { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitKeyword(this);
    }
}