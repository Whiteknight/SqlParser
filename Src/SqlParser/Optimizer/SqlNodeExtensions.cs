using SqlParser.Ast;

namespace SqlParser.Optimizer
{
    public static class SqlNodeExtensions
    {
        public static ISqlNode Optimize(this ISqlNode n)
        {
            if (n == null)
                return null;
            return new ExpressionOptimizeVisitor().Visit(n);
        }
    }
}
