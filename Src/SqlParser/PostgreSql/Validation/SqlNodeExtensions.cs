using SqlParser.SqlStandard;

namespace SqlParser.PostgreSql.Validation
{
    public static class SqlNodeExtensions
    {
        public static ValidationResult ValidateForPostgres(this ISqlNode n)
        {
            var result = new ValidationResult();
            // TODO: Pick the correct visitor based on node family
            var visitor = new SqlNodeValidateVisitor(result);
            visitor.Visit(n);
            return result;
        }
    }
}