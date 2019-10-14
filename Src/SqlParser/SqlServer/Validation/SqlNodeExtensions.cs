using SqlParser.Ast;

namespace SqlParser.SqlServer.Validation
{
    public static class SqlNodeExtensions
    {
        public static ValidationResult Validate(this ISqlNode n)
        {
            var result = new ValidationResult();
            // TODO: Pick the correct visitor based on node family
            var visitor = new SqlNodeValidateVisitor(result);
            visitor.Visit(n);
            return result;
        }
    }
}