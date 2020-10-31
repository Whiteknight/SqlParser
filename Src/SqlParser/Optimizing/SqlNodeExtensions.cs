namespace SqlParser.Optimizing
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
