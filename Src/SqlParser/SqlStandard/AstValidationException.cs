using System;
using System.Collections.Generic;
using System.Text;

namespace SqlParser.SqlStandard
{
    [Serializable]
    public class AstValidationException : Exception
    {
        public AstValidationException()
        {
        }

        public AstValidationException(string message) : base(message)
        {
        }

        public AstValidationException(string message, Exception inner) : base(message, inner)
        {
        }

        public IReadOnlyList<string> Errors { get; set; }

        public static AstValidationException Create(IReadOnlyList<string> errors)
        {
            var sb = new StringBuilder();
            sb.AppendLine("AST Validation failed");
            sb.AppendLine();
            foreach (var error in errors)
                sb.AppendLine(error);
            return new AstValidationException(sb.ToString());
        }
    }
}