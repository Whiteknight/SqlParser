using CastIron.SqlParsing.Ast;

namespace CastIron.SqlParsing.Optimizer
{
    public class ExpressionOptimizeVisitor : SqlNodeVisitor
    {
        public override SqlNode VisitCast(SqlCastNode n)
        {
            return n.TryReduce();
        }

        public override SqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            n = base.VisitInfixOperation(n) as SqlInfixOperationNode;
            if (n.IsArithmeticOperation())
            {
                if (n.Left is SqlNumberNode nl && n.Right is SqlNumberNode nr)
                {
                    var newNode = n.Operator.Apply(nl, nr);
                    newNode.Location = n.Location;
                    return newNode;
                }

                if (n.Left is SqlStringNode sl && n.Right is SqlStringNode a)
                {
                    var newNode = n.Operator.Apply(sl, a);
                    newNode.Location = n.Location;
                    return newNode;
                }

            }
            // TODO: If the operation is AND or OR and both sides can reduce to one of (TRUE, FALSE), we can reduce the whole thing

            return n;
        }

        public override SqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            // First make sure we visit the children nodes and reduce those as much as possible
            n = base.VisitPrefixOperation(n) as SqlPrefixOperationNode;
            if (n.Right is SqlNumberNode number)
            {
                if (n.Operator.Operator == "-")
                    return number.MakeNegative();
                if (n.Operator.Operator == "+")
                    return n.Right;
                if (n.Operator.Operator == "~")
                    return number.BitwiseInvert();
            }

            return n;
        }

        public override SqlNode VisitSelect(SqlSelectNode n)
        {
            // TODO: if SELECT WHERE can reduce to TRUE, the where clause can be omitted
            // TODO: If SELECT WHERE can reduce to FALSE, the entire statement can be replaced with an empty result set
            return base.VisitSelect(n);
        }

        
    }
}
