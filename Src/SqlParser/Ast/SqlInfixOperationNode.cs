using System.Collections.Generic;
using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlInfixOperationNode : SqlNode, ISqlNode
    {
        public ISqlNode Left { get; set; }
        public SqlOperatorNode Operator { get; set; }
        public ISqlNode Right { get; set; }

        private static readonly HashSet<string> _queryUnionOperators = new HashSet<string>
        {
            "UNION",
            "UNION ALL",
            "EXCEPT",
            "INTERSECT"
        };

        private static readonly HashSet<string> _booleanOperators = new HashSet<string>
        {
            "AND",
            "OR"
        };

        private static readonly HashSet<string> _arithmeticOperators = new HashSet<string>
        {
            "+", "-", "/", "*", "%", "&", "^", "|"
        };

        private static readonly HashSet<string> _comparisonOperators = new HashSet<string>
        {
            ">", "<", "=", "<=", ">=", "!=", "<>"
        };

        private static readonly HashSet<string> _comparisonSetModifiers = new HashSet<string>
        {
            "ALL", "ANY", "SOME"
        };

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitInfixOperation(this);

        

        public Location Location { get; set; }

        public SqlInfixOperationNode Update(ISqlNode left, SqlOperatorNode op, ISqlNode right)
        {
            if (left == Left && op == Operator && right == Right)
                return this;
            return new SqlInfixOperationNode
            {
                Location = Location,
                Left = left,
                Operator = op,
                Right = right
            };
        }

        public bool IsUnionOperation()
        {
            return _queryUnionOperators.Contains(Operator?.Operator);
        }

        public bool IsBooleanOperation()
        {
            return _booleanOperators.Contains(Operator?.Operator);
        }

        public bool IsArithmeticOperation()
        {
            return _arithmeticOperators.Contains(Operator?.Operator);
        }

        public bool IsComparisonOperation()
        {
            var op = Operator?.Operator;
            if (string.IsNullOrEmpty(op))
                return false;
            if (_comparisonOperators.Contains(op))
                return true;
            if (!op.Contains(" "))
                return false;
            var parts = op.Split(' ');
            return parts.Length == 2 && _comparisonOperators.Contains(parts[0]) && _comparisonSetModifiers.Contains(parts[1]);
        }

        public bool IsNullCompareOperator()
        {
            if (Operator.Operator == "IS" || Operator.Operator == "IS NOT")
                return true;
            return false;
        }
    }
}
