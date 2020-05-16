using System;
using ParserObjects;
using SqlParser.Tokenizing;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlNumberNode : SqlNode, ISqlNode
    {
        private enum DataType
        {
            Numeric,
            Int,
            BigInt
        }

        private readonly DataType _type;

        public SqlNumberNode()
        {
            _type = DataType.Int;
        }

        public SqlNumberNode(SqlToken token)
        {
            Location = token.Location;
            if (token.Value.Contains("."))
            {
                Numeric = decimal.Parse(token.Value);
                _type = DataType.Numeric;
            }
            else
            {
                var value = long.Parse(token.Value);
                if (value > int.MaxValue)
                {
                    Bigint = value;
                    _type = DataType.BigInt;
                }
                else
                {
                    Int = (int) value;
                    _type = DataType.Int;
                }
            }
        }

        public SqlNumberNode(decimal value)
        {
            Numeric = value;
            _type = DataType.Numeric;
        }

        public SqlNumberNode(int value)
        {
            Int = value;
            _type = DataType.Int;
        }

        public SqlNumberNode(long value)
        {
            Bigint = value;
            _type = DataType.BigInt;
        }

        public decimal Numeric { get; }
        public int Int { get;  }
        public long Bigint { get; }

        public override string ToString()
        {
            switch (_type)
            {
                case DataType.BigInt: return Bigint.ToString();
                case DataType.Int: return Int.ToString();
                case DataType.Numeric: return Numeric.ToString();
                default: throw new Exception("Unknown data type");
            }
        }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitNumber(this);

        public Location Location { get; set; }

        public bool IsNumeric => _type == DataType.Numeric;
        public bool IsInt => _type == DataType.Int;
        public bool IsBigint => _type == DataType.BigInt;

        public SqlNumberNode MakeNegative()
        {
            switch(_type)
            {
                case DataType.BigInt: return new SqlNumberNode(-Bigint);
                case DataType.Int: return new SqlNumberNode(-Int);
                case DataType.Numeric: return new SqlNumberNode(-Numeric);
                default: throw new Exception("Unknown data type");
            }
        }

        public SqlNumberNode BitwiseInvert()
        {
            switch (_type)
            {
                case DataType.BigInt: return new SqlNumberNode(~Bigint);
                case DataType.Int: return new SqlNumberNode(~Int);
                case DataType.Numeric: throw new Exception("Cannot bitwise invert a NUMERIC");
                default: throw new Exception("Unknown data type");
            }
        }

        public SqlNumberNode AsNumeric()
        {
            if (_type == DataType.Numeric)
                return this;
            if (_type == DataType.BigInt)
                return new SqlNumberNode((decimal) Bigint);
            if (_type == DataType.Int)
                return new SqlNumberNode((decimal) Int);
            throw new Exception("Unknown data type");
        }

        public SqlNumberNode AsBigInt()
        {
            if (_type == DataType.Numeric)
                throw new Exception("Cannot convert numeric to bigint");
            if (_type == DataType.BigInt)
                return this;
            if (_type == DataType.Int)
                return new SqlNumberNode((long)Int);
            throw new Exception("Unknown data type");
        }

        public SqlNumberNode AsInt()
        {
            if (_type == DataType.Numeric)
                throw new Exception("Cannot convert numeric to int");
            if (_type == DataType.BigInt)
                throw new Exception("Cannot convert bigint to int");
            if (_type == DataType.Int)
                return this;
            throw new Exception("Unknown data type");
        }
    }
}