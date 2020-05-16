using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;

namespace SqlParser
{
    public interface IDiagnosable
    {
        IEnumerable<ParseError> Errors { get; }
        void AddErrors(IEnumerable<ParseError> errors);
    }

    public static class DiagnosableExtensions
    {
        public static TElement AddErrors<TElement>(this TElement diagnosable, Location l, IEnumerable<string> errors)
            where TElement : IDiagnosable
        {
            if (diagnosable == null || errors == null)
                return diagnosable;

            var errorObjects = errors.Select(e => new ParseError(l, e));
            diagnosable.AddErrors(errorObjects);
            return diagnosable;
        }

        public static TElement AddErrors<TElement>(this TElement diagnosable, Location l, params string[] errors)
            where TElement : IDiagnosable
        {
            if (diagnosable == null)
                return diagnosable;
            if (errors == null || errors.Length == 0)
                return diagnosable;


            var errorObjects = errors.Select(e => new ParseError(l, e));
            diagnosable.AddErrors(errorObjects);
            return diagnosable;
        }
    }
}