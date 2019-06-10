using CastIron.SqlParsing.Ast;

namespace CastIron.SqlParsing.Optimizer
{
    public class ExpressionOptimizeVisitor : SqlNodeVisitor
    {
        public override SqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            // First make sure we visit the children nodes and reduce those as much as possible
            n = base.VisitPrefixOperation(n) as SqlPrefixOperationNode;
            if (n.Right is SqlNumberNode number)
            {
                if (n.Operator.Operator == "-")
                    return new SqlNumberNode(-number.Value) { Location = n.Location };
                if (n.Operator.Operator == "+")
                    return n.Right;
                if (n.Operator.Operator == "~")
                    return new SqlNumberNode((decimal)(~(int)number.Value)) { Location = n.Location };
            }

            return n;
        }

        public override SqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            n = base.VisitInfixOperation(n) as SqlInfixOperationNode;
            if (n.Left is SqlNumberNode left && n.Right is SqlNumberNode right)
            {
                switch (n.Operator.Operator)
                {
                    case "+": return new SqlNumberNode(left.Value + right.Value) { Location = n.Location };
                    case "-": return new SqlNumberNode(left.Value - right.Value) { Location = n.Location };
                    case "*": return new SqlNumberNode(left.Value * right.Value) { Location = n.Location };
                    case "/": return new SqlNumberNode(left.Value / right.Value) { Location = n.Location };
                    case "%": return new SqlNumberNode((int)left.Value % (int)right.Value) { Location = n.Location };
                    case "&": return new SqlNumberNode((int)left.Value & (int)right.Value) { Location = n.Location };
                    case "^": return new SqlNumberNode((int)left.Value ^ (int)right.Value) { Location = n.Location };
                    case "|": return new SqlNumberNode((int)left.Value | (int)right.Value) { Location = n.Location };
                }
            }

            return n;
        }
    }
}
