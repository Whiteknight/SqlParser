using SqlParser.SqlStandard;
using SqlParser.Validation;

namespace SqlParser.SqlServer.Validation
{
    public static class SqlNodeExtensions
    {
        public static ValidationResult ValidateForSqlServer(this ISqlNode n)
        {
            var result = new ValidationResult();
            // TODO: Pick the correct visitor based on node family
            var diagnosticsVisitor = new DiagnosticsVisitor(result);
            diagnosticsVisitor.Visit(n);
            var visitor = new SqlNodeValidateVisitor(result);
            visitor.Visit(n);
            return result;
        }
    }
}