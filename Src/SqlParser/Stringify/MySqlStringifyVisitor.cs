using System.Text;
using SqlParser.Ast;

namespace SqlParser.Stringify
{
    // TODO: Factor this better and don't inherit from SqlServer visitor
    public class MySqlStringifyVisitor : SqlServerStringifyVisitor
    {
        public MySqlStringifyVisitor(StringBuilder sb) 
            : base(sb)
        {
        }

        public override SqlNode VisitWithCte(SqlWithCteNode n)
        {
            if (n.Recursive)
                Append("RECURSIVE ");
            return base.VisitWithCte(n);
        }
    }
}