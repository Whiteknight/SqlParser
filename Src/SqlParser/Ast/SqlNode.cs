using System.Collections.Generic;

namespace SqlParser.Ast
{
    public class SqlNode : IDiagnosable
    {
        private List<ParseError> _errors;

        public IEnumerable<ParseError> Errors => _errors;

        public void AddErrors(IEnumerable<ParseError> errors)
        {
            if (errors == null)
                return;
            if (_errors == null)
                _errors = new List<ParseError>();
            _errors.AddRange(errors);
        }
    }
}
