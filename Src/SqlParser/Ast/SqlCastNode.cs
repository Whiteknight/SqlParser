using System.Linq;
using ParserObjects;
using SqlParser.Visiting;

namespace SqlParser.Ast
{
    public class SqlCastNode : SqlNode, ISqlNode
    {
        public ISqlNode Expression { get; set; }
        public SqlDataTypeNode DataType { get; set; }

        public ISqlNode Accept(INodeVisitorTyped visitor) => visitor.VisitCast(this);

        

        public Location Location { get; set; }

        public SqlCastNode Update(ISqlNode expr, SqlDataTypeNode dt)
        {
            if (expr == Expression && dt == DataType)
                return this;
            return new SqlCastNode
            {
                Location = Location,
                Expression = expr,
                DataType = dt
            };
        }

        public int[] GetSize()
        {
            if (DataType.Size is SqlKeywordNode asKeyword && asKeyword.Keyword == "MAX")
                return new [] { int.MaxValue };
            if (DataType.Size is SqlNumberNode asNumber)
                return new[] { asNumber.AsInt().Int };
            if (DataType.Size is SqlListNode<SqlNumberNode> asList)
                return asList.Children.Select(x => x.AsInt().Int).ToArray();
            return null;
        }

        public ISqlNode TryReduce()
        {
            if (DataType.DataType.Keyword == "CHAR" || DataType.DataType.Keyword == "VARCHAR")
            {
                var sizes = GetSize();
                if (sizes == null || !sizes.Any())
                    return this;
                return TryReduceToString(sizes[0]);
            }

            if (Expression is SqlStringNode asString)
                return TryReduceStringToNumber(asString);

            return this;
        }

        private ISqlNode TryReduceStringToNumber(SqlStringNode asString)
        {
            switch (DataType.DataType.Keyword)
            {
                case "INT":
                    return new SqlNumberNode(int.Parse(asString.Value));
                case "BIT":
                    return new SqlNumberNode(int.Parse(asString.Value) > 0 ? 1 : 0);
                case "BIGINT":
                    return new SqlNumberNode(long.Parse(asString.Value));
                case "DECIMAL":
                case "DEC":
                case "NUMERIC":
                    // TODO: size and precision
                    return new SqlNumberNode(decimal.Parse(asString.Value));
                default:
                    return this;
            }
        }

        private ISqlNode TryReduceToString(int size)
        {
            if (Expression is SqlStringNode asString)
            {
                var strValue = asString.Value;
                if (DataType.DataType.Keyword == "CHAR" && strValue.Length < size)
                    strValue = strValue.PadRight(size);
                if (strValue.Length > size)
                    strValue = strValue.Substring(0, size);
                if (strValue == asString.Value)
                    return asString;
                return new SqlStringNode(strValue) { Location = Location };
            }

            if (Expression is SqlNumberNode asNumber)
            {
                var strValue = asNumber.ToString();
                if (strValue.Length > size)
                    strValue = strValue.Substring(0, size);
                if (DataType.DataType.Keyword == "CHAR" && strValue.Length < size)
                    strValue = strValue.PadRight(size);

                return new SqlStringNode(strValue) { Location = Location };
               
            }
            return this;
        }
    }
}