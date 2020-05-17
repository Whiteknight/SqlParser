using System;
using System.Collections.Generic;
using System.Linq;
using ParserObjects;
using SqlParser.Ast;

namespace SqlParser.SqlStandard
{
    public class ValidationError
    {
        public ValidationError(string error, Location location, string source)
        {
            Error = error;
            Location = location;
            Source = source;
        }

        public string Error { get; }
        public Location Location { get; }
        public string Source { get; }

        public override string ToString()
        {
            if (Location == null)
                return $"{Source}: {Error}";
            return $"{Source}: {Error} at {Location}";
        }
    }

    public class ValidationResult
    {
        private readonly List<ValidationError> _errors;

        public ValidationResult()
        {
            _errors = new List<ValidationError>();
        }

        public bool AddError(ISqlNode parent, string name, string message)
        {
            _errors.Add(new ValidationError(message, null, $"{parent.GetType().Name}.{name}"));
            return false;
        }

        public bool AddError(string parserName, string message, Location location)
        {
            _errors.Add(new ValidationError(message, location, parserName));
            return false;
        }

        public void ThrowOnError()
        {
            if (_errors.Count > 1)
                throw AstValidationException.Create(_errors);
        }

        public bool Passed => _errors.Count == 0;

        public bool AssertNotNull(ISqlNode parent, string name, ISqlNode child)
        {
            if (child == null)
                return AddError(parent, name, "cannot be null");

            return true;
        }

        public bool AssertIsScalarExpression(ISqlNode parent, string name, ISqlNode child)
        {
            if (!AssertNotNull(parent, name, child))
                return false;
            var childType = child.GetType();
            // TODO: Fix this, needs to accept parenthesis, etc
            if (!new[]
            {
                typeof(SqlNumberNode), typeof(SqlNullNode), typeof(SqlInfixOperationNode),
                typeof(SqlPrefixOperationNode), typeof(SqlStringNode),
                typeof(SqlIdentifierNode), typeof(SqlQualifiedIdentifierNode), typeof(SqlVariableNode),
                typeof(SqlCastNode)
            }.Contains(childType))
                return AddError(parent, name, "Is not an acceptable scalar expression node");

            return true;
        }

        public bool AssertIsScalarExpressionOrNothing(ISqlNode parent, string name, ISqlNode child)
        {
            if (child == null)
                return true;
            return AssertIsScalarExpression(parent, name, child);
        }

        public bool AssertIsBooleanExpression(ISqlNode parent, string name, ISqlNode child)
        {
            if (!AssertNotNull(parent, name, child))
                return false;
            // TODO: Fix this
            //if (child is SqlInfixOperationNode childExpr)
            //{
            //    if (childExpr.IsComparisonOperation() || childExpr.IsBooleanOperation())
            //        return true;
            //}

            //return AddError(parent, name, "Is not an acceptable boolean expression node");
            return true;
        }

        public bool AssertIsUnionStatement(ISqlNode parent, string name, ISqlNode child)
        {
            if (!AssertNotNull(parent, name, child))
                return false;
            // TODO: Fix this
            if (child is SqlInfixOperationNode childExpr && childExpr.IsUnionOperation())
                return true;

            if (child is SqlSelectNode)
                return true;

            return AddError(parent, name, "Is not an acceptable query statement node");
        }

        public bool AssertIsValue(ISqlNode parent, string name, string value, string expected)
        {
            if (value != expected)
                return AddError(parent, name, $"Value {value} does not equal {expected}");

            return true;
        }

        public bool AssertIsPositiveNumber(ISqlNode parent, string name, decimal value)
        {
            if (value < 0M)
                return AddError(parent, name, $"Value {value} is not a positive value");

            return true;
        }

        public bool AssertIsNotEmpty<TNode>(ISqlNode parent, string name, SqlListNode<TNode> list)
            where TNode : class, ISqlNode
        {
            if (list.Count == 0)
                return AddError(parent, name, "List is empty");
            return true;
        }

        public bool AssertIsNotNullOrEmpty(ISqlNode parent, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return AddError(parent, name, "string value cannot be null or empty");
            return true;
        }

        public bool AssertIsNotNullOrEmpty<TNode>(ISqlNode parent, string name, SqlListNode<TNode> list) 
            where TNode : class, ISqlNode
        {
            if (list == null || list.Count == 0)
                return AddError(parent, name, "List node may not be null or empty");
            return true;
        }

        public bool UnexpectedNodeType(ISqlNode parent, string name, ISqlNode child)
        {
            return AddError(parent, name, $"{child.GetType().Name} is an unexpected node type");
        }

        public bool Assert(ISqlNode parent, string name, Func<bool> predicate)
        {
            if (!predicate())
                return AddError(parent, name, "Condition is false");
            return true;
        }
    }
}