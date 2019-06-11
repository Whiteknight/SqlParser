using CastIron.SqlParsing.Ast;

namespace CastIron.SqlParsing.Validation
{
    public static class SqlNodeExtensions
    {
        public static ValidationResult Validate(this SqlNode n)
        {
            var result = new ValidationResult();
            var visitor = new SqlNodeValidateVisitor(result);
            visitor.Visit(n);
            return result;
        }
    }
}