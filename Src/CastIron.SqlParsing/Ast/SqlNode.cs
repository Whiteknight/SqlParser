using System.Text;
using CastIron.SqlParsing.Optimizer;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing.Ast
{
    public abstract class SqlNode
    {
        public abstract SqlNode Accept(SqlNodeVisitor visitor);

        public abstract void ToString(SqlStringifier sb);

        public override string ToString()
        {
            var sb = new StringBuilder();
            ToString(new SqlStringifier(sb));
            return sb.ToString();
        }

        public Location Location { get; set; }

        public SqlNode Optimize()
        {
            return new ExpressionOptimizeVisitor().Visit(this);
        }
    }
}