namespace SqlParser.Ast
{
    public interface ISqlNodeVisitor
    {
        SqlNode Visit(SqlNode n);
    }
}