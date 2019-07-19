using System.Collections.Generic;
using System.Linq;
using SqlParser.Ast;

namespace SqlParser.Validation
{
    public class ValidationResult
    {
        private readonly List<string> _errors;

        public ValidationResult()
        {
            _errors = new List<string>();
        }

        private bool AddError(SqlNode parent, string name, string message)
        {
            _errors.Add($"{parent.GetType().Name}.{name}: {message}");
            return false;
        }

        public void ThrowOnError()
        {
            if (_errors.Count > 1)
                throw AstValidationException.Create(_errors);
        }

        public bool Passed => _errors.Count == 0;

        public bool AssertNotNull(SqlNode parent, string name, SqlNode child)
        {
            if (child == null)
                return AddError(parent, name, "cannot be null");

            return true;
        }

        public bool AssertIsScalarExpression(SqlNode parent, string name, SqlNode child)
        {
            if (!AssertNotNull(parent, name, child))
                return false;
            var childType = child.GetType();
            // TODO: Fix this
            if (!new[]
            {
                typeof(SqlNumberNode), typeof(SqlNullNode), typeof(SqlInfixOperationNode),
                typeof(SqlPrefixOperationNode), typeof(SqlStringNode),
                typeof(SqlIdentifierNode), typeof(SqlQualifiedIdentifierNode), typeof(SqlVariableNode)
            }.Contains(childType))
                return AddError(parent, name, "Is not an acceptable scalar expression node");

            return true;
        }

        public bool AssertIsBooleanExpression(SqlNode parent, string name, SqlNode child)
        {
            if (!AssertNotNull(parent, name, child))
                return false;
            // TODO: Fix this
            if (child is SqlInfixOperationNode childExpr)
            {
                if (childExpr.IsComparisonOperation() || childExpr.IsBooleanOperation())
                    return true;
            }

            return AddError(parent, name, "Is not an acceptable boolean expression node");
        }

        public bool AssertIsUnionStatement(SqlNode parent, string name, SqlNode child)
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

        public bool AssertIsValue(SqlNode parent, string name, string value, string expected)
        {
            if (value != expected)
                return AddError(parent, name, $"Value {value} does not equal {expected}");

            return true;
        }

        public bool AssertIsPositiveNumber(SqlNode parent, string name, decimal value)
        {
            if (value < 0M)
                return AddError(parent, name, $"Value {value} is not a positive value");

            return true;
        }

        public bool AssertIsNotEmpty<TNode>(SqlNode parent, string name, SqlListNode<TNode> list)
            where TNode : SqlNode
        {
            if (list.Count == 0)
                return AddError(parent, name, "List is empty");
            return true;
        }

        public bool AssertIsNotNullOrEmpty(SqlNode parent, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return AddError(parent, name, "string value cannot be null or empty");
            return true;
        }

        public bool UnexpectedNodeType(SqlNode parent, string name, SqlNode child)
        {
            return AddError(parent, name, $"{child.GetType().Name} is an unexpected node type");
        }
    }
}