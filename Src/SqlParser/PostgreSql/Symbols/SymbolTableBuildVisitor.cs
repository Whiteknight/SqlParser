using System.Collections.Generic;
using SqlParser.Ast;
using SqlParser.Symbols;
using SqlParser.Visiting;

namespace SqlParser.PostgreSql.Symbols
{
    // TODO: Postgres doesn't use @ to indicate variables, they are bare identifiers. (though parameters from,
    // for example, Npgsql, will use @ in their names). We will have to rely on the symbol table to keep
    // track of which identifiers are variables and which are some other kind of object.
    public class SymbolTableBuildVisitor : SqlNodeVisitor
    {
        private readonly Stack<SymbolTable> _tables;

        public SymbolTableBuildVisitor(IEnumerable<SymbolInfo> environmentalSymbols)
        {
            _tables = new Stack<SymbolTable>();
            _tables.Push(new SymbolTable(environmentalSymbols));
        }

        private SymbolTable PushSymbolTable()
        {
            var table = new SymbolTable(_tables.Peek());
            _tables.Push(table);
            return table;
        }

        private void PopSymbolTable()
        {
            _tables.Pop();
        }

        private SymbolTable Current => _tables.Peek();

        public override ISqlNode VisitAlias(SqlAliasNode n)
        {
            Current.AddSymbol(n.Alias.Name, new SymbolInfo
            {
                OriginalName = n.Alias.Name,
                OriginKind = SymbolOriginKind.Alias,
                DefinedAt = n.Location
            });
            return base.VisitAlias(n);
        }

        public override ISqlNode VisitWithCte(SqlWithCteNode n)
        {
            Current.AddSymbol(n.Name.Name, new SymbolInfo
            {
                OriginalName = n.Name.Name,
                OriginKind = SymbolOriginKind.UserDeclared,
                ObjectKind = ObjectKind.TableExpression,
                DefinedAt = n.Location
            });
            return base.VisitWithCte(n);
        }

        public override ISqlNode VisitDeclare(SqlDeclareNode n)
        {
            // It's a variable node, which is only supported in other dialects. Setup a translation
            if (n.Variable is SqlVariableNode v)
            {
                var symbolInfo = new SymbolInfo
                {
                    OriginalName = v.Name,
                    DefinedAt = n.Location,
                    Translate = x => new SqlIdentifierNode(((SqlVariableNode)x).GetBareName(), x.Location),
                    OriginKind = SymbolOriginKind.UserDeclared
                };

                // Add the original version and also add the translated version, so we can find both
                Current.AddSymbol(v.Name, symbolInfo);
                Current.AddSymbol(v.Name, new SymbolInfo
                {
                    OriginalName = v.Name,
                    DefinedAt = n.Location,
                    CreatedFrom = symbolInfo,
                    OriginKind = SymbolOriginKind.UserDeclared,
                });
                base.VisitDeclare(n);
            }

            var id = n.Variable as SqlIdentifierNode;
            Current.AddSymbol(id.Name, new SymbolInfo
            {
                OriginalName = id.Name,
                DefinedAt = id.Location,
                OriginKind = SymbolOriginKind.UserDeclared
            });
            return base.VisitDeclare(n);
        }

        public override ISqlNode VisitDelete(SqlDeleteNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitDelete(n) as SqlDeleteNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override ISqlNode VisitFunctionCall(SqlFunctionCallNode n)
        {
            if (n.Name is SqlKeywordNode)
                return base.VisitFunctionCall(n);

            if (n.Name is SqlIdentifierNode id)
            {
                Current.GetInfoOrThrow(id.ToString(), n.Location).InvokedLikeFunction(n.Location);
                return base.VisitFunctionCall(n);
            }

            if (n.Name is SqlObjectIdentifierNode objId)
            {
                Current.GetInfoOrThrow(objId.ToString(), n.Location).InvokedLikeFunction(n.Location);
                return base.VisitFunctionCall(n);
            }

            if (n.Name is SqlQualifiedIdentifierNode qid)
            {
                Current.GetInfoOrThrow(qid.ToString(), n.Location).InvokedLikeFunction(n.Location);
                return base.VisitFunctionCall(n);
            }

            return base.VisitFunctionCall(n);
        }

        public override ISqlNode VisitIdentifier(SqlIdentifierNode n)
        {
            Current.GetInfoOrThrow(n.Name, n.Location);
            return n;
        }

        public override ISqlNode VisitInfixOperation(SqlInfixOperationNode n)
        {
            if (n.IsUnionOperation())
                return base.VisitInfixOperation(n);
            ConfirmExpressionOperands(n.Left);
            ConfirmExpressionOperands(n.Right);
            return base.VisitInfixOperation(n);
        }

        private void ConfirmExpressionOperands(ISqlNode n)
        {
            if (n is SqlIdentifierNode id)
            {
                Current.GetInfoOrThrow(id.ToString(), n.Location).UsedInScalarExpression(n.Location);
                return;
            }

            // Object ID is a table name, only use the name of the table as the symbol, not the full qualification
            if (n is SqlObjectIdentifierNode objId)
            {
                Current.GetInfoOrThrow(objId.ToString(), n.Location).UsedInScalarExpression(n.Location);
                return;
            }

            if (n is SqlQualifiedIdentifierNode qid)
            {
                Current.GetInfoOrThrow(qid.ToString(), n.Location).UsedInScalarExpression(n.Location);
                return;
            }

            if (n is SqlVariableNode variable)
            {
                // This is a parameter from a parameterized query, so it must be defined beforehand
                Current.GetInfoOrThrow(variable.ToString(), n.Location).UsedInScalarExpression(n.Location);
                return;
            }
        }

        public override SqlJoinNode VisitJoin(SqlJoinNode n)
        {
            AddTableIds(n.Left);
            AddTableIds(n.Right);
            return base.VisitJoin(n);
        }

        private void AddTableIds(ISqlNode n)
        {
            // Identifier is presumed to be table name
            if (n is SqlIdentifierNode id)
            {
                // n could be a user-defined table variable, so check to see if it exists first
                var existing = Current.GetInfo(id.ToString());
                if (existing != null)
                {
                    existing.UsedAsTableExpression(n.Location);
                    return;
                }

                // It's not pre-defined, so we'll assume it's an environmental table and add it to the
                // current scope
                Current.AddSymbol(id.ToString(), new SymbolInfo
                {
                    OriginalName = id.ToString(),
                    OriginKind = SymbolOriginKind.Environmental,
                    ObjectKind = ObjectKind.TableExpression,
                    DefinedAt = id.Location
                });
                return;
            }

            // Object ID is a table name, only use the name of the table as the symbol, not the full qualification
            if (n is SqlObjectIdentifierNode objId)
            {
                Current.AddSymbol(objId.ToString(), new SymbolInfo
                {
                    OriginalName = objId.ToString(),
                    OriginKind = SymbolOriginKind.Environmental,
                    ObjectKind = ObjectKind.TableExpression,
                    DefinedAt = objId.Location
                });
            }

        }

        public override ISqlNode VisitObjectIdentifier(SqlObjectIdentifierNode n)
        {
            // Treat the object identifier as a single atomic value for the purposes of
            // the symbol table
            // TODO: How to handle correctly? We might refer to x.Id in one place but just
            // "Id" later so if we just see "Id" we might want to also see anything that
            // ends in ".Id" (and if more than one, we might want to complain about being
            // unable to pick)
            Current.GetInfoOrThrow(n.ToString(), n.Location);
            return n;
        }

        public override ISqlNode VisitPrefixOperation(SqlPrefixOperationNode n)
        {
            ConfirmExpressionOperands(n.Right);
            return base.VisitPrefixOperation(n);
        }

        public override ISqlNode VisitQualifiedIdentifier(SqlQualifiedIdentifierNode n)
        {
            // Treat the qualified identifier as a single atomic value for the purposes of
            // the symbol table
            // TODO: How to handle correctly? We might refer to x.Id in one place but just
            // "Id" later so if we just see "Id" we might want to also see anything that
            // ends in ".Id" (and if more than one, we might want to complain about being
            // unable to pick)
            Current.GetInfoOrThrow(n.ToString(), n.Location);
            return n;
        }

        public override ISqlNode VisitSelect(SqlSelectNode n)
        {
            var symbols = PushSymbolTable();
            // Visit the FROM clause to get all the source table expressions
            AddTableIds(n.FromClause);
            Visit(n.FromClause);

            // Visit the columns to get all column names and aliases
            foreach (var column in n.Columns)
            {
                if (column is SqlAliasNode alias)
                {
                    Current.AddSymbol(alias.Alias.Name, new SymbolInfo
                    {
                        OriginalName = alias.Alias.Name,
                        OriginKind = SymbolOriginKind.Alias,
                        ObjectKind = ObjectKind.Scalar,
                        DefinedAt = alias.Location
                    });
                }

                // Identifiers are presumed to be column names from tables in the FROM list
                if (column is SqlIdentifierNode id)
                {
                    // TODO: Should check the symbol table first to see if we have this 
                    // identifier defined.
                    Current.AddSymbol(id.ToString(), new SymbolInfo
                    {
                        OriginalName = id.ToString(),
                        OriginKind = SymbolOriginKind.UserDeclared,
                        ObjectKind = ObjectKind.Scalar,
                        DefinedAt = id.Location
                    });
                }

                // Qualified IDs are presumed <tableOrAlias>.<column>
                if (column is SqlQualifiedIdentifierNode qid)
                {
                    // Make sure the table name is already defined in the FROM clause
                    Current.GetInfoOrThrow(qid.Qualifier.Name, n.Location);
                    // We want <table>.<column>, not <table>.*
                    // TODO: Would like to be able to look it up by short-name and fully-qualified name
                    if (qid.Identifier is SqlIdentifierNode nested)
                    {
                        Current.AddSymbol(nested.Name, new SymbolInfo
                        {
                            OriginalName = qid.ToString(),
                            OriginKind = SymbolOriginKind.Environmental,
                            ObjectKind = ObjectKind.Scalar,
                            DefinedAt = qid.Location
                        });
                    }
                }

                // Variable should already be defined
                // TODO: We'll need to pre-populate the symbol table with out-of-scope parameter
                // declarations so we can make sure those are handled correctly.
                if (column is SqlVariableNode v)
                    Current.GetInfoOrThrow(v.Name, n.Location);
            }
            Visit(n.WhereClause);
            Visit(n.GroupByClause);
            Visit(n.HavingClause);
            Visit(n.OrderByClause);

            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override ISqlNode VisitSet(SqlSetNode n)
        {
            // If the LHS is an identifier, ensure that it is defined already.
            if (n.Variable is SqlIdentifierNode id)
            {
                // TODO: Mark that it's the target of an assignment?
                Current.GetInfoOrThrow(id.Name, n.Location);
                return base.VisitSet(n);
            }

            // Otherwise just do normal visit. All the parts will be accounted for later
            return base.VisitSet(n);
        }

        public override ISqlNode VisitStatementList(SqlStatementListNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitStatementList(n) as SqlStatementListNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override ISqlNode VisitUpdate(SqlUpdateNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitUpdate(n) as SqlUpdateNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override ISqlNode VisitWith(SqlWithNode n)
        {
            var symbols = PushSymbolTable();
            n = base.VisitWith(n) as SqlWithNode;
            n.Symbols = symbols;
            PopSymbolTable();
            return n;
        }

        public override ISqlNode VisitVariable(SqlVariableNode n)
        {
            // @ variables are only used as parameters in parameterized queries, so if they are defined
            // at all they must have been defined environmentally. Check if the symbol exists already. If
            // so it's a parameter and we leave it as-is. Otherwise it's a misnamed variable (probably
            // output from a parser of a different dialect) so we need to setup a translation rule if one
            // doesn't exist already.
            var info = Current.GetInfo(n.Name);
            if (info != null)
                return n;

            // Add this symbol with a translation rule
            var originalSymbol = new SymbolInfo {
                OriginalName = n.Name,
                Translate = x => new SqlIdentifierNode(((SqlVariableNode)x).GetBareName(), x.Location)
            };
            Current.AddSymbol(n.Name, originalSymbol);

            // Add a shadow symbol, with the correct name, just in case
            Current.AddSymbol(n.GetBareName(), new SymbolInfo
            {
                OriginalName = n.Name,
                CreatedFrom = originalSymbol
            });
            return n;
        }
    }
}
