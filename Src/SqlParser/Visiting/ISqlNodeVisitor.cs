using SqlParser.Ast;

namespace SqlParser.Visiting
{
    public interface ISqlNodeVisitor
    {
        ISqlNode Visit(ISqlNode n);
    }
}