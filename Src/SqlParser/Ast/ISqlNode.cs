using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public interface ISqlNode
    {
        Location Location { get; set; }
        ISqlNode Accept(INodeVisitorTyped visitor);
    }
}
