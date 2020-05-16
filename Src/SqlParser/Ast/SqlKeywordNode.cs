using ParserObjects;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlKeywordNode : SqlNode, ISqlNode
    {
        public SqlKeywordNode()
        {
        }

        public SqlKeywordNode(SqlToken t)
        {
            Keyword = t.Value;
            Location = t.Location;
            AddErrors(t.Errors);
        }

        public SqlKeywordNode(string keyword, Location l = null)
        {
            Keyword = keyword;
            Location = l;
        }

        public string Keyword { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitKeyword(this);

        public Location Location { get; set; }
    }
}