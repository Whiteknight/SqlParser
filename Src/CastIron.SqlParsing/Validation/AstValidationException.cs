using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace CastIron.SqlParsing.Validation
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

        protected AstValidationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
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