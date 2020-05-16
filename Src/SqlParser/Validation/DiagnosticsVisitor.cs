using SqlParser.PostgreSql.Validation;
using SqlParser.Visiting;

namespace SqlParser.Validation
{
    public class DiagnosticsVisitor : SqlNodeVisitor
    {
        private readonly ValidationResult _result;

        public DiagnosticsVisitor(ValidationResult result)
        {
            _result = result;
        }

        public override ISqlNode Visit(ISqlNode n)
        {
            if (n.Errors != null)
            {
                foreach (var error in n.Errors)
                    _result.AddError(n, "Errors", $"{error.Location}: {error.ErrorMessage}");
            }

            return base.Visit(n);
        }
    }
}
