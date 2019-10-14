using System;
using SqlParser.Ast;
using SqlParser.Tokenizing;

namespace SqlParser.PostgreSql.Parsing
{
    public partial class Parser
    {
        private ISqlNode ParseNumberOrVariable(Tokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            throw ParsingException.CouldNotParseRule(nameof(ParseNumberOrVariable), next);
        }

        private SqlNumberNode ParseNumber(Tokenizer t)
        {
            var next = t.GetNext();
            if (next.IsType(SqlTokenType.Number))
                return new SqlNumberNode(next);

            t.PutBack(next);
            return null;
        }

        private ISqlNode ParseVariableOrConstant(Tokenizer t)
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
            throw ParsingException.CouldNotParseRule(nameof(ParseVariableOrConstant), next);
        }

        private ISqlNode ParseVariableOrQualifiedIdentifier(Tokenizer t)
        {
            var next = t.GetNext();

            // <Variable>
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            t.PutBack(next);
            var identifier = ParseQualifiedIdentifier(t);
            if (identifier != null)
                return identifier;

            throw ParsingException.CouldNotParseRule(nameof(ParseVariableOrQualifiedIdentifier), next);
        }

        private SqlIdentifierNode ParseIdentifier(Tokenizer t)
        {
            var next = t.Expect(SqlTokenType.Identifier);
            return new SqlIdentifierNode(next);
        }

        private ISqlNode ParseQualifiedIdentifier(Tokenizer t)
        {
            // ( <Qualifier> "." )? <Identifier>
            
            var next = t.Peek();
            if (!next.IsType(SqlTokenType.Identifier) && !next.IsType(SqlTokenType.Keyword))
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
                Identifier = identifier.Is(SqlTokenType.Symbol, "*") ? (ISqlNode)new SqlOperatorNode("*") : new SqlIdentifierNode(identifier)
            };
        }

        private ISqlNode ParseVariableOrObjectIdentifier(Tokenizer t)
        {
            var next = t.GetNext();

            // <Variable>
            if (next.IsType(SqlTokenType.Variable))
                return new SqlVariableNode(next);
            t.PutBack(next);
            return ParseObjectIdentifier(t);
        }

        private SqlObjectIdentifierNode ParseObjectIdentifier(Tokenizer t)
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

        private ISqlNode ParseMaybeAliasedScalar(Tokenizer t, Func<Tokenizer, ISqlNode> parse)
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

        private ISqlNode ParseMaybeAliasedTable(Tokenizer t, Func<Tokenizer, ISqlNode> parse)
        {
            var node = parse(t);

            var next = t.Peek();
            SqlToken aliasToken = null;
            Location location = null;
            if (next.IsKeyword("AS"))
            {
                location = t.GetNext().Location;
                aliasToken = t.GetIdentifierOrKeyword();
            }
            else if (next.IsType(SqlTokenType.Identifier))
                aliasToken = t.GetNext();

            if (aliasToken == null)
                return node;

            var alias = new SqlAliasNode
            {
                Location = location ?? aliasToken.Location,
                Source = node,
                Alias = new SqlIdentifierNode(aliasToken)
            };

            if (t.Peek().IsSymbol("("))
                alias.ColumnNames = ParseParenthesis(t, x => ParseList(x, ParseIdentifier)).Expression;

            return alias;
        }

        private SqlStringNode ParseString(Tokenizer t)
        {
            var s = t.Expect(SqlTokenType.QuotedString);
            return new SqlStringNode(s);
        }
    }
}
