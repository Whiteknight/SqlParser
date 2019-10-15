using System;
using SqlParser.Ast;

namespace SqlParser.Symbols
{
    public class SymbolInfo
    {
        public string OriginalName { get; set; }
        public Func<ISqlNode, ISqlNode> Translate { get; set; }

        public Location DefinedAt { get; set; }
        public SymbolOriginKind OriginKind { get; set; }
        public ObjectKind ObjectKind { get; set; }

        public SymbolInfo CreatedFrom { get; set; }

        public ISqlNode Resolve(ISqlNode n)
        {
            return Translate?.Invoke(n) ?? n;
        }

        public void InvokedLikeFunction(Location l)
        {
            if (ObjectKind == ObjectKind.Unknown)
                ObjectKind = ObjectKind.Invokable;
            if (ObjectKind == ObjectKind.Invokable)
                return;
            throw SymbolWrongObjectKindException.NonInvokableInvoked(this, l);
        }

        public void AssignedTo(Location l)
        {
            // TODO: What to do here?
        }

        //public void MemberIsAccess(Location l)
        //{
        //    if (ObjectKind == ObjectKind.Unknown)
        //        ObjectKind = ObjectKind.TableExpression;
        //    if (ObjectKind == ObjectKind.TableExpression)
        //        return;
        //    throw SymbolWrongObjectKindException.NonDataSourceMemberAccessed(this, l);
        //}

        public void UsedInScalarExpression(Location l)
        {
            if (ObjectKind == ObjectKind.Unknown)
                ObjectKind = ObjectKind.Scalar;
            if (ObjectKind == ObjectKind.Scalar)
                return;
            throw SymbolWrongObjectKindException.NonScalarUsedInExpression(this, l);
        }

        public void UsedAsTableExpression(Location l)
        {
            if (ObjectKind == ObjectKind.Unknown)
                ObjectKind = ObjectKind.TableExpression;
            if (ObjectKind == ObjectKind.TableExpression)
                return;
            throw SymbolWrongObjectKindException.NonTableUsedAsTableExpression(this, l);
        }
    }
}