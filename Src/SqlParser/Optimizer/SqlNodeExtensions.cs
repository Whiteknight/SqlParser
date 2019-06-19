using SqlParser.Ast;

namespace SqlParser.Optimizer
{
    public static class SqlNodeExtensions
    {
        public static SqlNode Optimize(this SqlNode n)
        {
            if (n == null)
                return null;
            return new ExpressionOptimizeVisitor().Visit(n);
        }
    }
}
