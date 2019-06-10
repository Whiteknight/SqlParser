﻿using System;
using CastIron.SqlParsing.Ast;
using CastIron.SqlParsing.Tokenizing;

namespace CastIron.SqlParsing
{
    public partial class SqlParser
    {
        private SqlNode ParseNumberOrVariable(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            throw new Exception($"Expecting number or variable but found {next}");
        }

        private SqlNumberNode ParseNumber(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            t.PutBack(next);
            return null;
        }

        private SqlNode ParseNumberOrKeyword(SqlTokenizer t)
        {
            var next = t.GetNext();

            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            if (next.IsType(SqlTokenType.Keyword))
                return new SqlKeywordNode(next);

            return null;
        }

        private SqlNode ParseVariableConstantOrQualifiedIdentifier(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.QuotedString))
                return new SqlStringNode(next);
            t.PutBack(next);
            var number = ParseNumber(t);
            if (number != null)
                return number;
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier != null)
                return identifier;

            throw new Exception("Cannot parse variable, constant or qualified identifier");
        }

        private SqlNode ParseVariableOrConstant(SqlTokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.QuotedString))
                return new SqlStringNode(next);
            t.PutBack(next);
            var number = ParseNumber(t);
            if (number != null)
                return number;
            throw new Exception("Cannot parse variable or constant");
        }

        private SqlNode ParseVariableOrQualifiedIdentifier(SqlTokenizer t)
        {
            var next = t.GetNext();

            // <Variable>
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            t.PutBack(next);
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier != null)
                return identifier;

            throw new Exception("Cannot parse variable or identifier");
        }

        private SqlIdentifierNode ParseIdentifier(SqlTokenizer t)
        {
            var next = t.Expect(SqlTokenType.Identifier);
            return new SqlIdentifierNode(next);
        }

        private SqlNode ParseQualifiedIdentifier(SqlTokenizer t)
        {
            // ( <Qualifier> "." )? <Identifier>
            
            var next = t.Peek();
            if (!next.IsType(SqlTokenType.Identifier))
                return null;
            t.GetNext();
            if (!t.Peek().Is(SqlTokenType.Symbol, "."))
                return new SqlIdentifierNode(next);
            t.GetNext();
            var qualifier = new SqlIdentifierNode(next);
            var identifier = t.GetNext();
            return new SqlQualifiedIdentifierNode
            {
                Location = qualifier.Location,
                Qualifier = qualifier,
                Identifier = identifier.Is(SqlTokenType.Symbol, "*") ? (SqlNode)new SqlOperatorNode("*") : new SqlIdentifierNode(identifier)
            };
        }

        private SqlNode ParseVariableOrObjectIdentifier(SqlTokenizer t)
        {
            var next = t.GetNext();

            // <Variable>
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            t.PutBack(next);
            return ParseObjectIdentifier(t);
        }

        private SqlObjectIdentifierNode ParseObjectIdentifier(SqlTokenizer t)
        {
            // (((<ServerName> ".")? <DatabaseName> ".")? <Schema> ".")? <Identifier>
            var item1 = t.Expect(SqlTokenType.Identifier);
            if (!t.NextIs(SqlTokenType.Symbol, ".", true))
            {
                return new SqlObjectIdentifierNode
                {
                    Location = item1.Location,
                    Name = new SqlIdentifierNode(item1)
                };
            }

            var item2 = t.Expect(SqlTokenType.Identifier);
            if (!t.NextIs(SqlTokenType.Symbol, ".", true))
            {
                return new SqlObjectIdentifierNode
                {
                    Location = item1.Location,
                    Schema = new SqlIdentifierNode(item1),
                    Name = new SqlIdentifierNode(item2)
                };
            }

            var item3 = t.Expect(SqlTokenType.Identifier);
            if (!t.NextIs(SqlTokenType.Symbol, ".", true))
            {
                return new SqlObjectIdentifierNode
                {
                    Location = item1.Location,
                    Database = new SqlIdentifierNode(item1),
                    Schema = new SqlIdentifierNode(item2),
                    Name = new SqlIdentifierNode(item3)
                };
            }

            var item4 = t.Expect(SqlTokenType.Identifier);
            return new SqlObjectIdentifierNode
            {
                Location = item1.Location,
                Server = new SqlIdentifierNode(item1),
                Database = new SqlIdentifierNode(item2),
                Schema = new SqlIdentifierNode(item3),
                Name = new SqlIdentifierNode(item4)
            };
        }

        private SqlNode ParseMaybeAliased(SqlTokenizer t, Func<SqlTokenizer, SqlNode> parse)
        {
            var node = parse(t);

            var next = t.Peek();
            if (next.IsKeyword("AS"))
            {
                var asToken = t.GetNext();
                var alias = t.Expect(SqlTokenType.Identifier);
                return new SqlAliasNode
                {
                    Location = asToken.Location,
                    Source = node,
                    Alias = new SqlIdentifierNode(alias)
                };
            }
            if (next.IsType(SqlTokenType.Identifier))
            {
                t.GetNext();
                return new SqlAliasNode
                {
                    Location = node.Location,
                    Source = node,
                    Alias = new SqlIdentifierNode(next)
                };
            }

            return node;
        }

        private SqlStringNode ParseString(SqlTokenizer t)
        {
            var s = t.Expect(SqlTokenType.QuotedString);
            return new SqlStringNode(s);
        }
    }
}
