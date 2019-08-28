using System;
using SqlParser.Tokenizing;

namespace SqlParser.Ast
{
    public class SqlOperatorNode  : SqlNode
    {
        public SqlOperatorNode()
        {
        }

        public SqlOperatorNode(SqlToken token)
        {
            Operator = token.Value;
            Location = token.Location;
        }

        public SqlOperatorNode(string op, Location location)
        {
            Operator = op;
            Location = location;
        }

        public SqlOperatorNode(string op)
        {
            Operator = op;
        }

        public string Operator { get; set; }

        public override SqlNode Accept(ISqlNodeVisitImplementation visitor) => visitor.VisitOperator(this);

        public SqlStringNode Apply(SqlStringNode left, SqlStringNode right)
        {
            if (Operator == "+")
                return new SqlStringNode(left.Value + right.Value);
            throw new Exception($"Cannot apply operator {Operator} to operands of type string");
        }

        public SqlNumberNode Apply(SqlNumberNode left, SqlNumberNode right)
        {
            if (left.IsNumeric || right.IsNumeric)
                return ApplyNumeric(left.AsNumeric(), right.AsNumeric());
            if (left.IsBigint || right.IsBigint)
                return ApplyBigint(left.AsBigInt(), right.AsBigInt());
            return ApplyInt(left, right);
        }

        private SqlNumberNode ApplyNumeric(SqlNumberNode left, SqlNumberNode right)
        {
            switch (Operator)
            {
                case "+": return new SqlNumberNode(left.Numeric + right.Numeric);
                case "-": return new SqlNumberNode(left.Numeric - right.Numeric);
                case "*": return new SqlNumberNode(left.Numeric * right.Numeric);
                case "/": return new SqlNumberNode(left.Numeric / right.Numeric);
                case "%": return new SqlNumberNode(left.Numeric % right.Numeric);
            }

            throw new Exception($"Cannot apply operator {Operator} to operands of type numeric");
        }

        private SqlNumberNode ApplyInt(SqlNumberNode left, SqlNumberNode right)
        {
            switch (Operator)
            {
                case "+": return new SqlNumberNode(left.Int + right.Int);
                case "-": return new SqlNumberNode(left.Int - right.Int);
                case "*": return new SqlNumberNode(left.Int * right.Int);
                case "/": return new SqlNumberNode(left.Int / right.Int);
                case "%": return new SqlNumberNode(left.Int % right.Int);
                case "&": return new SqlNumberNode(left.Int & right.Int);
                case "^": return new SqlNumberNode(left.Int ^ right.Int);
                case "|": return new SqlNumberNode(left.Int | right.Int);
            }

            throw new Exception($"Cannot apply operator {Operator} to operands of type int");
        }

        private SqlNumberNode ApplyBigint(SqlNumberNode left, SqlNumberNode right)
        {
            switch (Operator)
            {
                case "+": return new SqlNumberNode(left.Bigint + right.Bigint);
                case "-": return new SqlNumberNode(left.Bigint - right.Bigint);
                case "*": return new SqlNumberNode(left.Bigint * right.Bigint);
                case "/": return new SqlNumberNode(left.Bigint / right.Bigint);
                case "%": return new SqlNumberNode(left.Bigint % right.Bigint);
                case "&": return new SqlNumberNode(left.Bigint & right.Bigint);
                case "^": return new SqlNumberNode(left.Bigint ^ right.Bigint);
                case "|": return new SqlNumberNode(left.Bigint | right.Bigint);
            }

            throw new Exception($"Cannot apply operator {Operator} to operands of type bigint");
        }
    }
}